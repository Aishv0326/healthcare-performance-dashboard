import { Paper, Typography, Stack, Box } from "@mui/material";

export default function KpiCard({ title, value, suffix = "", icon = null }) {
  return (
    <Paper
      elevation={0}
      sx={{
        p: 2,
        borderRadius: 2,
        position: "relative",
        overflow: "hidden",
        boxShadow: "0 6px 18px rgba(0,0,0,0.08)",
      }}
    >
      {/* left accent */}
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
        <Box>
          <Typography variant="subtitle2" sx={{ opacity: 0.75 }}>
            {title}
          </Typography>
          <Typography variant="h5" sx={{ fontWeight: 800, mt: 0.5 }}>
            {value}
            {suffix}
          </Typography>
        </Box>

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
