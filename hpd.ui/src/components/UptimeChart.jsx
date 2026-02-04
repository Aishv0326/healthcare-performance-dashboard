import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

/**
 * UptimeChart
 *
 * Renders a time-series line chart showing service uptime percentage.
 *
 * Props:
 * - data: Array of objects with shape:
 *   {
 *     time: string,
 *     uptime: number
 *   }
 */
export default function UptimeChart({ data }) {
  return (
    // Responsive container ensures chart scales with screen size
    <ResponsiveContainer width="100%" height={300}>
      <LineChart data={data}>
        {/* Time buckets on X-axis */}
        <XAxis dataKey="time" />

        {/* Percentage scale on Y-axis */}
        <YAxis />

        {/* Hover tooltip for precise values */}
        <Tooltip />

        {/* Green line represents healthy uptime */}
        <Line type="monotone" dataKey="uptime" stroke="#2e7d32" />
      </LineChart>
    </ResponsiveContainer>
  );
}
