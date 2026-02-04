using Hpd.Api.Data;
using Microsoft.EntityFrameworkCore;
using Hpd.Api.Services;


var builder = WebApplication.CreateBuilder(args);

// Register EF Core with SQL Server LocalDB
// LocalDB is used to keep the project fully runnable without cloud dependencies
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

// Add controllers (API endpoints)
builder.Services.AddControllers();

// Add controllers (API endpoints)builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Register background service that continuously generates demo metrics
// This ensures the dashboard always has live data
builder.Services.AddHostedService<MetricGeneratorService>();
// Configure CORS to allow React dev server access
builder.Services.AddCors(options =>
{
    options.AddPolicy("ui", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Apply CORS policy before routing
app.UseCors("ui");

// Map controller routes
app.MapControllers();

app.Run();
