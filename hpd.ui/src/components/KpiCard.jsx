import { Paper, Typography, Stack, Box } from "@mui/material";

/**
 * KpiCard
 *
 * Reusable KPI card component used to display high-level metrics
 * such as request count, latency, error rate, and uptime.
 *
 * Props:
 * - title: Label shown at the top of the card
 * - value: Main numeric value displayed
 * - suffix: Optional unit (e.g. "ms", "%")
 * - icon: Optional visual icon shown on the right
 * - onClick: Optional click handler (used for telemetry or drill-down actions)
 */
export default function KpiCard({
  title,
  value,
  suffix = "",
  icon = null,
  onClick,
}) {
  return (
    <Paper
      elevation={0}
      onClick={onClick}
      sx={{
        p: 2,
        borderRadius: 2,
        position: "relative",
        overflow: "hidden",
        boxShadow: "0 6px 18px rgba(0,0,0,0.08)",

        // Enable hover and pointer cursor only if the card is clickable
        cursor: onClick ? "pointer" : "default",
        transition: "transform 0.15s ease",
        "&:hover": onClick ? { transform: "translateY(-2px)" } : {},
      }}
    >
      {/* Decorative left accent bar to visually group KPI cards */}
      <Box
        sx={{
          position: "absolute",
          left: 0,
          top: 0,
          bottom: 0,
          width: 6,
          bgcolor: "primary.main",
        }}
      />

      <Stack direction="row" justifyContent="space-between" alignItems="center">
        {/* KPI text content */}
        <Box>
          <Typography variant="subtitle2" sx={{ opacity: 0.75 }}>
            {title}
          </Typography>

          <Typography variant="h5" sx={{ fontWeight: 800, mt: 0.5 }}>
            {value}
            {suffix}
          </Typography>
        </Box>

        {/* Optional icon container */}
        {icon ? (
          <Box
            sx={{
              width: 44,
              height: 44,
              borderRadius: 2,
              bgcolor: "rgba(96,163,217,0.18)",
              display: "grid",
              placeItems: "center",
            }}
          >
            {icon}
          </Box>
        ) : null}
      </Stack>
    </Paper>
  );
}
