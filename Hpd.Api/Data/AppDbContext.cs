using Hpd.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Hpd.Api.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<MetricEvent> MetricEvents { get; set; }
    }
}
