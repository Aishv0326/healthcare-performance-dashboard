using Hpd.Api.Data;
using Hpd.Api.Models;

namespace Hpd.Api.Services
{
    public class MetricGeneratorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Random _random = new();

        private static readonly string[] Endpoints =
        {
            "/api/patients",
            "/api/appointments",
            "/api/labs/results",
            "/api/billing/claims",
            "/api/providers",
            "/api/auth/login",
            "/api/alerts"
        };

        // Only these endpoints are impacted during incidents (more realistic)
        private static readonly HashSet<string> IncidentEndpoints = new()
        {
            "/api/labs/results",
            "/api/billing/claims"
        };

        private bool _incidentActive = false;
        private DateTime _incidentEndTime;

        public MetricGeneratorService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                GenerateIncidentIfNeeded();
                await GenerateBatch();
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }

        private void GenerateIncidentIfNeeded()
        {
            // 5% chance to start an incident
            if (!_incidentActive && _random.Next(1, 100) <= 5)
            {
                _incidentActive = true;
                _incidentEndTime = DateTime.UtcNow.AddMinutes(5);
            }

            if (_incidentActive && DateTime.UtcNow > _incidentEndTime)
            {
                _incidentActive = false;
            }
        }

        private async Task GenerateBatch()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            for (int i = 0; i < 20; i++)
            {
                var endpoint = Endpoints[_random.Next(Endpoints.Length)];

                // Incident applies only to selected endpoints
                var incidentHitsThisEndpoint = _incidentActive && IncidentEndpoints.Contains(endpoint);

                bool isError;
                int latency;

                if (incidentHitsThisEndpoint)
                {
                    latency = _random.Next(800, 2000);
                    isError = _random.Next(1, 100) <= 20;
                }
                else
                {
                    latency = _random.Next(100, 400);
                    isError = _random.Next(1, 100) <= 2;
                }

                // Optional: make some endpoints naturally slower/faster
                var endpointExtraLatency = endpoint switch
                {
                    "/api/labs/results" => _random.Next(100, 300),
                    "/api/billing/claims" => _random.Next(150, 400),
                    "/api/auth/login" => _random.Next(50, 150),
                    _ => _random.Next(0, 120)
                };

                var metric = new MetricEvent
                {
                    TimestampUtc = DateTime.UtcNow,
                    Endpoint = endpoint,
                    LatencyMs = latency + endpointExtraLatency,
                    IsError = isError,
                    StatusCode = isError ? 500 : 200,
                    IsUp = !isError
                };

                db.MetricEvents.Add(metric);
            }

            await db.SaveChangesAsync();
        }
    }
}
