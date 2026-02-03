import {
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  TableContainer,
} from "@mui/material";

export default function TopEndpointsTable({ rows }) {
  return (
    <TableContainer sx={{ overflowX: "auto" }}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Endpoint</TableCell>
            <TableCell>Requests</TableCell>
            <TableCell>Avg Latency</TableCell>
            <TableCell>P95</TableCell>
            <TableCell>Error %</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {rows.map((r, i) => (
            <TableRow key={i}>
              <TableCell>{r.endpoint}</TableCell>
              <TableCell>{r.requests}</TableCell>
              <TableCell>{r.avgLatencyMs}</TableCell>
              <TableCell>{r.p95LatencyMs}</TableCell>
              <TableCell>{r.errorRatePct}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
