namespace Hpd.Api.Dtos
{
    /// <summary>
    /// Time-series response used for trend charts in the dashboard.
    ///
    /// Instead of returning nested objects per timestamp, this DTO
    /// uses parallel arrays to keep payloads compact and chart-friendly.
    ///
    /// All lists are aligned by index:
    /// TimestampsUtc[i] corresponds to AvgLatencyMs[i], ErrorRatePct[i], etc.
    /// </summary>
    public class TrendsResponse
    {
        /// <summary>
        /// Time range requested by the client (e.g. "24h", "7d").
        /// Returned for reference/debugging.
        /// </summary>
        public string Range { get; set; } = "24h";

        /// <summary>
        /// Bucket size used for aggregation (e.g. "5m", "15m").
        /// Returned for reference/debugging.
        /// </summary>
        public string Bucket { get; set; } = "5m";

        /// <summary>
        /// Start timestamp (UTC) for each bucket.
        /// Used as the X-axis for charts.
        /// </summary>
        public List<DateTime> TimestampsUtc { get; set; } = new();

        /// <summary>
        /// Average latency (ms) per bucket.
        /// </summary>
        public List<int> AvgLatencyMs { get; set; } = new();

        /// <summary>
        /// Error rate percentage per bucket.
        /// </summary>
        public List<double> ErrorRatePct { get; set; } = new();

        /// <summary>
        /// Uptime percentage per bucket.
        /// </summary>
        public List<double> UptimeRatePct { get; set; } = new();

        /// <summary>
        /// Requests per minute (RPM) per bucket.
        ///
        /// Useful for visualizing traffic spikes and load patterns.
        /// </summary>
        public List<double> RequestsPerMinute { get; set; } = new();
    }
}
