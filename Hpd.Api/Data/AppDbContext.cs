using Hpd.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Hpd.Api.Data
{
    /// <summary>
    /// Entity Framework Core database context for the application.
    ///
    /// This class represents the session with the database and provides
    /// strongly-typed access to tables via DbSet properties.
    ///
    /// In this project, MetricEvents is the core table that stores
    /// simulated request/health metrics consumed by the dashboard.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// DbContext is configured via dependency injection in Program.cs.
        /// The connection string points to SQL Server LocalDB for local development.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Stores one row per simulated request/metric event.
        /// Used by summary/trends/top-endpoints endpoints to compute KPIs.
        /// </summary>
        public DbSet<MetricEvent> MetricEvents { get; set; }
    }
}
