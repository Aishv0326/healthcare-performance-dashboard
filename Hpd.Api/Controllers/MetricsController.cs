using Hpd.Api.Data;
using Hpd.Api.Dtos;
using Hpd.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Hpd.Api.Controllers
{
    // API controller responsible for exposing performance metrics
    // consumed by the React dashboard.

    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MetricsController(AppDbContext db)
        {
            _db = db;
        }

        // Returns overall KPI summary for the given time window
        [HttpGet("summary")]
        public async Task<IActionResult> Summary([FromQuery] string range = "24h")
        {
            //var since = DateTime.UtcNow.AddHours(-hours);
            var rangeTs = TimeRangeParser.Parse(range, TimeSpan.FromHours(24));
            var since = DateTime.UtcNow - rangeTs;

            var query = _db.MetricEvents.Where(x => x.TimestampUtc >= since);

            var total = await query.CountAsync();
            if (total == 0)
            {
                return Ok(new
                {
                    range,
                    totalRequests = 0,
                    avgLatencyMs = 0,
                    errorRate = 0,
                    uptimeRate = 0
                });
            }

            var avgLatency = await query.AverageAsync(x => (double)x.LatencyMs);
            var errorCount = await query.CountAsync(x => x.IsError);
            var upCount = await query.CountAsync(x => x.IsUp);

            return Ok(new
            {
                range,
                totalRequests = total,
                avgLatencyMs = (int)Math.Round(avgLatency),
                errorRate = Math.Round((double)errorCount / total * 100, 2),
                uptimeRate = Math.Round((double)upCount / total * 100, 2)
            });
        }

        /// <summary>
        /// Returns time-series metrics for charts (latency, error rate, uptime, RPM)
        /// bucketed into fixed intervals (e.g., 5 minutes) over a time range (e.g., 24 hours).
        ///
        /// Important behavior:
        /// - We generate a full timeline from "since" -> now, so missing buckets are returned as zeros.
        ///   This keeps chart X-axis consistent and avoids broken / jagged graphs when no traffic exists.
        /// </summary>
        /// <param name="range">Time window to look back (examples: 30m, 24h, 7d). Default: 24h.</param>
        /// <param name="bucket">Bucket size for aggregation (examples: 1m, 5m, 15m). Default: 5m.</param>
        [HttpGet("trends")]
        public async Task<IActionResult> Trends([FromQuery] string range = "24h", [FromQuery] string bucket = "5m")
        {
            // Parse range and bucket into TimeSpan values with safe defaults.
            // This lets the frontend pass simple strings like "24h" or "5m".
            var rangeTs = TimeRangeParser.Parse(range, TimeSpan.FromHours(24));
            var bucketTs = TimeRangeParser.Parse(bucket, TimeSpan.FromMinutes(5));

            // Guardrails to avoid heavy queries / too many data points:
            // - minimum bucket = 1 minute
            // - maximum bucket = 1 hour
            if (bucketTs < TimeSpan.FromMinutes(1)) bucketTs = TimeSpan.FromMinutes(1);
            if (bucketTs > TimeSpan.FromHours(1)) bucketTs = TimeSpan.FromHours(1);

            var since = DateTime.UtcNow - rangeTs;

            // Load only the fields needed for aggregation.
            // NOTE: For very large datasets, this could be optimized to aggregate in SQL directly.
            var events = await _db.MetricEvents
                .Where(x => x.TimestampUtc >= since)
                .Select(x => new { x.TimestampUtc, x.LatencyMs, x.IsError, x.IsUp })
                .ToListAsync();

            // Floors a timestamp to the start of its bucket.
            // Example: 10:07 with a 5m bucket becomes 10:05.
            static DateTime FloorToBucket(DateTime dt, TimeSpan bucketSize)
            {
                var ticks = dt.Ticks - (dt.Ticks % bucketSize.Ticks);
                return new DateTime(ticks, DateTimeKind.Utc);
            }

            // Group actual events by bucket start time.
            // We store as a dictionary for O(1) lookup when filling the full timeline.
            var grouped = events
                .GroupBy(e => FloorToBucket(e.TimestampUtc, bucketTs))
                .ToDictionary(g => g.Key, g => g.ToList());

            // Build a complete bucket timeline so charts always have continuous X-axis points.
            // If no data exists for a bucket, we return zeros for that bucket.
            var timeline = new List<DateTime>();
            var cursor = FloorToBucket(since, bucketTs);
            var now = DateTime.UtcNow;

            while (cursor <= now)
            {
                timeline.Add(cursor);
                cursor = cursor.Add(bucketTs);
            }

            var resp = new TrendsResponse { Range = range, Bucket = bucket };

            foreach (var ts in timeline)
            {
                if (grouped.TryGetValue(ts, out var bucketEvents))
                {
                    var total = bucketEvents.Count;


                    // Aggregate metrics for this bucket:
                    // - avg latency (ms)
                    // - error rate (%)
                    // - uptime rate (%)
                    // - requests per minute (RPM)
                    var avgLatency = (int)Math.Round(bucketEvents.Average(x => (double)x.LatencyMs));
                    var errorRate = Math.Round(bucketEvents.Count(x => x.IsError) * 100.0 / total, 2);
                    var uptimeRate = Math.Round(bucketEvents.Count(x => x.IsUp) * 100.0 / total, 2);
                    var rpm = Math.Round(total / bucketTs.TotalMinutes, 2);

                    resp.TimestampsUtc.Add(ts);
                    resp.AvgLatencyMs.Add(avgLatency);
                    resp.ErrorRatePct.Add(errorRate);
                    resp.UptimeRatePct.Add(uptimeRate);
                    resp.RequestsPerMinute.Add(rpm);
                }
                else
                { 
                    // Missing bucket -> return zeros so charts remain stable and predictable.
                    resp.TimestampsUtc.Add(ts);
                    resp.AvgLatencyMs.Add(0);
                    resp.ErrorRatePct.Add(0);
                    resp.UptimeRatePct.Add(0);
                    resp.RequestsPerMinute.Add(0);
                }
            }

            return Ok(resp);
        }
        /// <summary>
        /// Returns per-endpoint performance ranking over a time range.
        /// Useful for identifying bottlenecks and endpoints that produce errors.
        ///
        /// Metrics returned per endpoint:
        /// - total requests
        /// - average latency
        /// - P95 latency (approx percentile using sorted list)
        /// - error rate (%)
        /// </summary>
        /// <param name="range">Time window to look back (examples: 30m, 24h, 7d). Default: 24h.</param>
        /// <param name="sort">Sort key (requests, latencyAvg, latencyP95, errorRate). Default: latencyP95.</param>
        /// <param name="take">Number of endpoints to return (bounded to 1..50).</param>

        [HttpGet("top-endpoints")]
        public async Task<IActionResult> TopEndpoints(
         [FromQuery] string range = "24h",
         [FromQuery] string sort = "latencyP95",
         [FromQuery] int take = 10)
        {
            // Limit output size to avoid huge payloads.
            if (take < 1) take = 10;
            if (take > 50) take = 50;

            var rangeTs = TimeRangeParser.Parse(range, TimeSpan.FromHours(24));
            var since = DateTime.UtcNow - rangeTs;

            // Pull only the minimal fields needed for endpoint aggregation.
            var events = await _db.MetricEvents
                .Where(x => x.TimestampUtc >= since)
                .Select(x => new { x.Endpoint, x.LatencyMs, x.IsError })
                .ToListAsync();

            // Helper to compute P95 latency:
            // - sort latencies and pick the 95th percentile index
            // NOTE: This is fine for demo scale; for large data you’d compute percentile in SQL or via approximation.
            static int P95(List<int> values)
            {
                if (values.Count == 0) return 0;
                values.Sort();
                var idx = (int)Math.Ceiling(0.95 * values.Count) - 1;
                idx = Math.Clamp(idx, 0, values.Count - 1);
                return values[idx];
            }

            // Group metrics by endpoint and compute summary stats.
            var items = events
                .GroupBy(x => x.Endpoint ?? "")
                .Select(g =>
                {
                    var latencies = g.Select(x => x.LatencyMs).ToList();
                    var total = g.Count();
                    var avg = (int)Math.Round(latencies.Average(v => (double)v));
                    var p95 = P95(latencies);
                    var errPct = total == 0 ? 0 : Math.Round(g.Count(x => x.IsError) * 100.0 / total, 2);

                    return new TopEndpointDto
                    {
                        Endpoint = g.Key,
                        Requests = total,
                        AvgLatencyMs = avg,
                        P95LatencyMs = p95,
                        ErrorRatePct = errPct
                    };
                });

            // Apply user-selected sorting.
            items = sort.ToLowerInvariant() switch
            {
                "requests" => items.OrderByDescending(x => x.Requests),
                "latencyavg" => items.OrderByDescending(x => x.AvgLatencyMs),
                "latencyp95" => items.OrderByDescending(x => x.P95LatencyMs),
                "errorrate" => items.OrderByDescending(x => x.ErrorRatePct),
                _ => items.OrderByDescending(x => x.P95LatencyMs)
            };

            return Ok(items.Take(take).ToList());
        }

        /// <summary>
        /// Clears all MetricEvents from the database.
        /// Useful for demos, testing, and resetting the dashboard to a clean state.
        /// </summary>
        [HttpDelete("reset")]
        public async Task<IActionResult> Reset()
        {
            _db.MetricEvents.RemoveRange(_db.MetricEvents);
            await _db.SaveChangesAsync();
            return Ok(new { message = "MetricEvents cleared" });
        }

    }
}


