using Hpd.Api.Data;
using Hpd.Api.Dtos;
using Hpd.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Hpd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MetricsController(AppDbContext db)
        {
            _db = db;
        }
       
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

        //[HttpGet("trends")]
        //public async Task<IActionResult> Trends([FromQuery] string range = "24h", [FromQuery] string bucket = "5m")
        //{
        //    var rangeTs = TimeRangeParser.Parse(range, TimeSpan.FromHours(24));
        //    var bucketTs = TimeRangeParser.Parse(bucket, TimeSpan.FromMinutes(5));

        //    // guardrails
        //    if (bucketTs < TimeSpan.FromMinutes(1)) bucketTs = TimeSpan.FromMinutes(1);
        //    if (bucketTs > TimeSpan.FromHours(1)) bucketTs = TimeSpan.FromHours(1);

        //    var since = DateTime.UtcNow - rangeTs;

        //    // pull only what we need to compute buckets
        //    var events = await _db.MetricEvents
        //        .Where(x => x.TimestampUtc >= since)
        //        .Select(x => new { x.TimestampUtc, x.LatencyMs, x.IsError, x.IsUp })
        //        .ToListAsync();

        //    // bucket start = floor(timestamp to bucket interval
        //    static DateTime FloorToBucket(DateTime dt, TimeSpan bucketSize)
        //    {
        //        var ticks = dt.Ticks - (dt.Ticks % bucketSize.Ticks);
        //        return new DateTime(ticks, DateTimeKind.Utc);
        //    }

        //    var grouped = events
        //        .GroupBy(e => FloorToBucket(e.TimestampUtc, bucketTs))
        //        .OrderBy(g => g.Key)
        //        .ToList();

        //    var resp = new TrendsResponse { Range = range, Bucket = bucket };

        //    foreach (var g in grouped)
        //    {
        //        var total = g.Count();
        //        if (total == 0) continue;

        //        var avgLatency = (int)Math.Round(g.Average(x => (double)x.LatencyMs));
        //        var errorRate = Math.Round(g.Count(x => x.IsError) * 100.0 / total, 2);
        //        var uptimeRate = Math.Round(g.Count(x => x.IsUp) * 100.0 / total, 2);

        //        // requests per minute for this bucket
        //        var rpm = Math.Round(total / bucketTs.TotalMinutes, 2);

        //        resp.TimestampsUtc.Add(g.Key);
        //        resp.AvgLatencyMs.Add(avgLatency);
        //        resp.ErrorRatePct.Add(errorRate);
        //        resp.UptimeRatePct.Add(uptimeRate);
        //        resp.RequestsPerMinute.Add(rpm);
        //    }

        //    return Ok(resp);
        //}
        [HttpGet("trends")]
        public async Task<IActionResult> Trends([FromQuery] string range = "24h", [FromQuery] string bucket = "5m")
        {
            var rangeTs = TimeRangeParser.Parse(range, TimeSpan.FromHours(24));
            var bucketTs = TimeRangeParser.Parse(bucket, TimeSpan.FromMinutes(5));

            if (bucketTs < TimeSpan.FromMinutes(1)) bucketTs = TimeSpan.FromMinutes(1);
            if (bucketTs > TimeSpan.FromHours(1)) bucketTs = TimeSpan.FromHours(1);

            var since = DateTime.UtcNow - rangeTs;

            var events = await _db.MetricEvents
                .Where(x => x.TimestampUtc >= since)
                .Select(x => new { x.TimestampUtc, x.LatencyMs, x.IsError, x.IsUp })
                .ToListAsync();

            static DateTime FloorToBucket(DateTime dt, TimeSpan bucketSize)
            {
                var ticks = dt.Ticks - (dt.Ticks % bucketSize.Ticks);
                return new DateTime(ticks, DateTimeKind.Utc);
            }

            // Group actual data
            var grouped = events
                .GroupBy(e => FloorToBucket(e.TimestampUtc, bucketTs))
                .ToDictionary(g => g.Key, g => g.ToList());

            // Generate full timeline
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
                    // Missing bucket → zeros
                    resp.TimestampsUtc.Add(ts);
                    resp.AvgLatencyMs.Add(0);
                    resp.ErrorRatePct.Add(0);
                    resp.UptimeRatePct.Add(0);
                    resp.RequestsPerMinute.Add(0);
                }
            }

            return Ok(resp);
        }

        [HttpGet("top-endpoints")]
        public async Task<IActionResult> TopEndpoints(
         [FromQuery] string range = "24h",
         [FromQuery] string sort = "latencyP95",
         [FromQuery] int take = 10)
        {
            if (take < 1) take = 10;
            if (take > 50) take = 50;

            var rangeTs = TimeRangeParser.Parse(range, TimeSpan.FromHours(24));
            var since = DateTime.UtcNow - rangeTs;

            var events = await _db.MetricEvents
                .Where(x => x.TimestampUtc >= since)
                .Select(x => new { x.Endpoint, x.LatencyMs, x.IsError })
                .ToListAsync();

            static int P95(List<int> values)
            {
                if (values.Count == 0) return 0;
                values.Sort();
                var idx = (int)Math.Ceiling(0.95 * values.Count) - 1;
                idx = Math.Clamp(idx, 0, values.Count - 1);
                return values[idx];
            }

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

        [HttpDelete("reset")]
        public async Task<IActionResult> Reset()
        {
            _db.MetricEvents.RemoveRange(_db.MetricEvents);
            await _db.SaveChangesAsync();
            return Ok(new { message = "MetricEvents cleared" });
        }

    }
}


