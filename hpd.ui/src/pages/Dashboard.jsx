import { useEffect, useState } from "react";
import { Grid, Container, Typography, Paper, Stack, Box } from "@mui/material";
import KpiCard from "../components/KpiCard";
import LatencyChart from "../components/LatencyChart";
import ErrorChart from "../components/ErrorChart";
import UptimeChart from "../components/UptimeChart";
import TopEndpointsTable from "../components/TopEndpointsTable";
import { getSummary, getTrends, getTopEndpoints } from "../api/client";
import { trackEvent, trackError } from "../monitoring/logger";

export default function Dashboard() {
  const [summary, setSummary] = useState(null);
  const [trendData, setTrendData] = useState([]);
  const [topEndpoints, setTopEndpoints] = useState([]);

  useEffect(() => {
    trackEvent("PageView", { page: "Dashboard" });

    async function load() {
      try {
        const s = await getSummary();
        const t = await getTrends();
        const top = await getTopEndpoints();

        setSummary(s);

        const chartData = t.timestampsUtc.map((time, i) => ({
          time: time.substring(11, 16),
          latency: t.avgLatencyMs[i],
          error: t.errorRatePct[i],
          uptime: t.uptimeRatePct[i],
        }));

        setTrendData(chartData);
        setTopEndpoints(top);

        trackEvent("DashboardDataLoaded", {
          totalRequests: s.totalRequests,
          points: chartData.length,
          endpoints: top.length,
        });
      } catch (err) {
        trackError(err, { where: "Dashboard.load" });
      }
    }

    load();
  }, []);

  if (!summary) {
    return (
      <Box sx={{ minHeight: "100vh", py: 4 }}>
        <Container>
          <Typography>Loading...</Typography>
        </Container>
      </Box>
    );
  }

  return (
    <Box sx={{ minHeight: "100vh", py: 4 }}>
      <Container>
        <Typography variant="h4" mb={3}>
          Healthcare Performance Dashboard
        </Typography>
        <Typography variant="body1" sx={{ opacity: 0.8, mb: 3 }}>
          Live performance view of API latency, uptime, and error rates (demo
          data generator running).
        </Typography>

        <Typography variant="h6" sx={{ mb: 1 }}>
          System Overview
        </Typography>

        <Grid container spacing={2}>
          <Grid item xs={12} sm={6} md={3}>
            <KpiCard
              title="Requests"
              value={summary.totalRequests}
              icon={<span>📦</span>}
              onClick={() => trackEvent("KPI_Click", { kpi: "Requests" })}
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <KpiCard
              title="Avg Latency"
              value={summary.avgLatencyMs}
              suffix=" ms"
              icon={<span>⏱️</span>}
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <KpiCard
              title="Error Rate"
              value={summary.errorRate}
              suffix=" %"
              icon={<span>⚠️</span>}
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <KpiCard
              title="Uptime"
              value={summary.uptimeRate}
              suffix=" %"
              icon={<span>✅</span>}
            />
          </Grid>
        </Grid>

        <Typography variant="h6" sx={{ mt: 3, mb: 1 }}>
          Performance Trends
        </Typography>
        <Stack spacing={2}>
          <Paper
            sx={{
              p: 2,
              mt: 3,
              borderRadius: 2,
              boxShadow: "0 6px 18px rgba(0,0,0,0.08)",
            }}
          >
            <Typography variant="h6" sx={{ mb: 1, fontWeight: 700 }}>
              Latency (Avg)
            </Typography>
            <LatencyChart data={trendData} />
          </Paper>

          <Paper
            sx={{
              p: 2,
              mt: 3,
              borderRadius: 2,
              boxShadow: "0 6px 18px rgba(0,0,0,0.08)",
            }}
          >
            <Typography variant="h6" sx={{ mb: 1, fontWeight: 700 }}>
              Error Rate (%)
            </Typography>
            <ErrorChart data={trendData} />
          </Paper>

          <Paper
            sx={{
              p: 2,
              mt: 3,
              borderRadius: 2,
              boxShadow: "0 6px 18px rgba(0,0,0,0.08)",
            }}
          >
            <Typography variant="h6" sx={{ mb: 1, fontWeight: 700 }}>
              Uptime (%)
            </Typography>
            <UptimeChart data={trendData} />
          </Paper>
        </Stack>

        <Paper
          sx={{
            p: 2,
            mt: 3,
            borderRadius: 2,
            boxShadow: "0 6px 18px rgba(0,0,0,0.08)",
          }}
        >
          <Typography variant="h6" sx={{ mb: 1, fontWeight: 700 }}>
            Top Endpoints (P95 / Error Rate)
          </Typography>

          <TopEndpointsTable rows={topEndpoints} />
        </Paper>
      </Container>
    </Box>
  );
}
