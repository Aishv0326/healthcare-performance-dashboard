namespace Hpd.Api.Dtos
{
    /// <summary>
    /// Represents aggregated performance metrics for a single API endpoint.
    ///
    /// This DTO is used by the "Top Endpoints" view in the dashboard to:
    /// - identify slow or problematic endpoints
    /// - compare request volume vs latency vs error rate
    ///
    /// The data is pre-aggregated on the server so the frontend
    /// can render tables without additional computation.
    /// </summary>
    public class TopEndpointDto
    {
        /// <summary>
        /// API route or endpoint name (e.g. /api/patients, /api/labs/results).
        /// </summary>
        public string Endpoint { get; set; } = "";

        /// <summary>
        /// Total number of requests received by this endpoint
        /// within the selected time range.
        /// </summary>
        public int Requests { get; set; }

        /// <summary>
        /// Average response latency (in milliseconds) for the endpoint.
        /// </summary>
        public int AvgLatencyMs { get; set; }

        /// <summary>
        /// 95th percentile latency (P95) in milliseconds.
        ///
        /// Used to highlight worst-case performance instead of averages,
        /// which is a common practice in production monitoring systems.
        /// </summary>
        public int P95LatencyMs { get; set; }

        /// <summary>
        /// Percentage of requests that resulted in an error (0–100).
        /// </summary>
        public double ErrorRatePct { get; set; }
    }
}
