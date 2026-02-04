namespace Hpd.Api.Models
{
    public class MetricEvent
    {
        public int Id { get; set; }

        // UTC timestamp when the request occurred
        public DateTime TimestampUtc { get; set; }
        // Service name 
        public string ServiceName { get; set; } = "api";
        // API endpoint name (e.g. /api/patients)
        public string Endpoint { get; set; } = string.Empty;

        // Response time in milliseconds
        public int LatencyMs { get; set; }

        // Indicates whether the request resulted in an error
        public bool IsError { get; set; }

        // HTTP status code (200, 500, etc.)
        public int StatusCode { get; set; }

        // Indicates whether the service was considered "up"
        public bool IsUp { get; set; }
    }
}
