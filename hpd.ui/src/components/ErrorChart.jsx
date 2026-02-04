import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

/**
 * ErrorChart
 *
 * Renders a line chart showing error rate trends over time.
 * This component is used in the dashboard to visualize
 * how frequently API requests fail within the selected time range.
 *
 * Props:
 * - data: Array of objects with the following shape:
 *   {
 *     time: string (formatted time label),
 *     error: number (error rate percentage)
 *   }
 */
export default function ErrorChart({ data }) {
  return (
    // ResponsiveContainer ensures the chart automatically
    // resizes based on its parent container (mobile + desktop friendly)
    <ResponsiveContainer width="100%" height={300}>
      <LineChart data={data}>
        {/* X-axis displays time buckets */}
        <XAxis dataKey="time" />

        {/* Y-axis displays error percentage values */}
        <YAxis />

        {/* Tooltip shows exact values on hover */}
        <Tooltip />

        {/* Line representing error rate (%) over time */}
        <Line
          type="monotone"
          dataKey="error"
          stroke="#d32f2f" // red color to visually indicate errors
        />
      </LineChart>
    </ResponsiveContainer>
  );
}
