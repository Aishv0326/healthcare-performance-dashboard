namespace Hpd.Api.Models
{
    public class MetricEvent
    {
        public int Id { get; set; }
        public DateTime TimestampUtc { get; set; }

        public string ServiceName { get; set; } = "api";
        public string Endpoint { get; set; } = "";

        public int LatencyMs { get; set; }
        public bool IsError { get; set; }
        public int StatusCode { get; set; }
        public bool IsUp { get; set; }
    }
}
