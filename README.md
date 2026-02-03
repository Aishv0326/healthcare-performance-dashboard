# Healthcare Performance Dashboard

A full-stack monitoring and observability system built with **ASP.NET Core** and **React**. This dashboard demonstrates how modern healthcare platforms track system performance, identify bottlenecks, and maintain reliability at scale.

The application simulates real-world API traffic patterns and provides actionable insights through interactive visualizations, KPI tracking, and endpoint analysis—all running locally with no external dependencies.

## Features

**Key Performance Indicators**

- Real-time request volume tracking
- Average latency monitoring
- Error rate detection
- Service uptime measurement

**Interactive Analytics**

- Historical latency trends
- Error pattern visualization
- Uptime timeline
- Top endpoint performance ranking

**Performance Analysis**

- Slow endpoint identification with P95 latency metrics
- Error rate breakdown by endpoint
- Continuous background data generation (every 15 seconds)
- Simulated incident scenarios for testing

## System Architecture

```
Metric Generator (Background Service)
         ↓
ASP.NET Core Web API (REST)
         ↓
SQL Server LocalDB (Data Store)
         ↓
React Dashboard (Material UI + Recharts)
```

## Technology Stack

### Backend

- **ASP.NET Core 8** - Web API framework
- **Entity Framework Core** - Object-relational mapping
- **SQL Server LocalDB** - Local database
- **Swagger/OpenAPI** - API documentation
- **Background Services** - Continuous metric generation

### Frontend

- **React 18** - UI library
- **Vite** - Build tool and dev server
- **Material UI (MUI)** - Component library
- **Recharts** - Charting library
- **Fetch API** - HTTP client

### Dependencies

**NuGet Packages**

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Swashbuckle.AspNetCore

**npm Packages**

- @mui/material
- @emotion/react & @emotion/styled
- recharts

---

## API Endpoints

All endpoints are accessed at `https://localhost:7011`.

| Method | Endpoint                                                       | Purpose                                             |
| ------ | -------------------------------------------------------------- | --------------------------------------------------- |
| GET    | `/api/health`                                                  | Health check status                                 |
| GET    | `/api/metrics/summary?hours=24`                                | Key performance indicators for specified time range |
| GET    | `/api/metrics/trends?range=24h&bucket=5m`                      | Historical trend data for visualization             |
| GET    | `/api/metrics/top-endpoints?range=24h&sort=latencyP95&take=10` | Slowest endpoints ranked by latency                 |
| POST   | `/api/metrics/reset`                                           | Clear all metrics and restart generation            |

---

## Getting Started

### Backend Setup

1. Navigate to the API project:

   cd Hpd.Api

2. Restore dependencies:

   dotnet restore

3. Create the database:

   dotnet ef database update

4. Start the API server:

   dotnet run

5. Access the API documentation at https://localhost:7011/swagger

### Frontend Setup

1. Navigate to the UI project:

   cd hpd.ui

2. Install dependencies:

   npm install

3. Start the development server:

   npm run dev

4. Access the dashboard at http://localhost:5173

## Client-Side Monitoring

The application includes a lightweight telemetry system that tracks user interactions and errors locally.

**Tracked Events**

- Dashboard page loads
- KPI card interactions
- Custom metric events
- Client-side errors

**Storage**

- Console logging for debugging
- LocalStorage persistence (`hpd_telemetry`)

**Implementation**

- Location: `hpd.ui/src/monitoring/logger.js`
- Can be extended to Azure Application Insights, OpenTelemetry, or Datadog

## Design Decisions

**Local Metrics Generation**

The dashboard continuously generates synthetic metrics in the background to ensure the UI always displays realistic data patterns. This approach eliminates external service dependencies while allowing the system to simulate real-world scenarios.

**Why Material UI?**

Material Design components provide professional styling and a robust component library, enabling rapid development of enterprise-grade interfaces without the overhead of custom CSS frameworks.

**Why No Cloud Services?**

This project prioritizes demonstrating core architecture and data flow patterns. Cloud integration can be added in the future without requiring major redesigns, as the API layer remains abstracted from infrastructure concerns.

## Future Enhancements

- Incident detection and alerting system
- User authentication and role-based access control
- Threshold-based alert rules
- Cloud deployment (Azure App Service, SQL Database)
- Integration with external observability platforms
- Real-time incident timeline view
- Custom dashboard creation

## Author

Aishvarya Shah  
Full-Stack Software Engineer | ASP.NET Core | React | System Design
