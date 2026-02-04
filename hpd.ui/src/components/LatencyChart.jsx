import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

/**
 * LatencyChart
 *
 * Displays average API latency over time as a line chart.
 * Used in the dashboard to visualize performance trends and spikes.
 *
 * Props:
 * - data: Array of objects with shape:
 *   {
 *     time: string,      // formatted time label (HH:mm)
 *     latency: number    // average latency in milliseconds
 *   }
 */
export default function LatencyChart({ data }) {
  return (
    // Responsive container ensures the chart scales with its parent
    <ResponsiveContainer width="100%" height={300}>
      <LineChart data={data}>
        {/* Time buckets on the X-axis */}
        <XAxis dataKey="time" />

        {/* Latency values (ms) on the Y-axis */}
        <YAxis />

        {/* Tooltip shows precise values on hover */}
        <Tooltip />

        {/* Blue line represents average latency */}
        <Line type="monotone" dataKey="latency" stroke="#0074B7" />
      </LineChart>
    </ResponsiveContainer>
  );
}
