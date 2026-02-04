import {
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  TableContainer,
} from "@mui/material";

/**
 * TopEndpointsTable
 *
 * Displays aggregated performance metrics per API endpoint
 * in a tabular format.
 *
 * Props:
 * - rows: Array of objects with shape:
 *   {
 *     endpoint: string,
 *     requests: number,
 *     avgLatencyMs: number,
 *     p95LatencyMs: number,
 *     errorRatePct: number
 *   }
 */
export default function TopEndpointsTable({ rows }) {
  return (
    // TableContainer enables horizontal scrolling on smaller screens
    <TableContainer sx={{ overflowX: "auto" }}>
      <Table>
        <TableHead>
          <TableRow>
            {/* API route name */}
            <TableCell>Endpoint</TableCell>

            {/* Total request count */}
            <TableCell>Requests</TableCell>

            {/* Average latency in milliseconds */}
            <TableCell>Avg Latency</TableCell>

            {/* 95th percentile latency (P95) */}
            <TableCell>P95</TableCell>

            {/* Error rate percentage */}
            <TableCell>Error %</TableCell>
          </TableRow>
        </TableHead>

        <TableBody>
          {rows.map((r, i) => (
            // Using index as key is acceptable here since rows are static snapshots
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
