namespace Hpd.Api.Dtos
{
    public class TrendsResponse
    {
        public string Range { get; set; } = "24h";
        public string Bucket { get; set; } = "5m";

        public List<DateTime> TimestampsUtc { get; set; } = new();
        public List<int> AvgLatencyMs { get; set; } = new();
        public List<double> ErrorRatePct { get; set; } = new();
        public List<double> UptimeRatePct { get; set; } = new();
        public List<double> RequestsPerMinute { get; set; } = new();
    }
}
