namespace Hpd.Api.Dtos
{
    public class TopEndpointDto
    {
        public string Endpoint { get; set; } = "";
        public int Requests { get; set; }
        public int AvgLatencyMs { get; set; }
        public int P95LatencyMs { get; set; }
        public double ErrorRatePct { get; set; }
    }
}
