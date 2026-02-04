// Base URL for the backend API (ASP.NET Core).
// Keeping this in one place makes it easy to change later (e.g., when deployed).
const BASE_URL = "https://localhost:7011";

/**
 * Fetch KPI summary data for a given time range.
 * Example: range = "24h"
 *
 * Returns JSON from:
 * GET /api/Metrics/summary?range=...
 */
export async function getSummary(range = "24h") {
    const res = await fetch(`${BASE_URL}/api/Metrics/summary?range=${range}`);
    return await res.json();
}

/**
 * Fetch trend data for charts.
 * Example: range = "24h", bucket = "5m"
 *
 * Returns JSON from:
 * GET /api/Metrics/trends?range=...&bucket=...
 */
export async function getTrends(range = "24h", bucket = "5m") {
    const res = await fetch(
        `${BASE_URL}/api/Metrics/trends?range=${range}&bucket=${bucket}`
    );
    return await res.json();
}

/**
 * Fetch top endpoints aggregation (used for the table).
 * Example: range = "24h", sort = "latencyP95", take = 10
 *
 * Returns JSON from:
 * GET /api/Metrics/top-endpoints?range=...&sort=...&take=...
 */
export async function getTopEndpoints(range = "24h", sort = "latencyP95", take = 10) {
    const url = new URL(`${BASE_URL}/api/Metrics/top-endpoints`);
    url.searchParams.append('range', range);
    url.searchParams.append('sort', sort);
    url.searchParams.append('take', take.toString());

    const res = await fetch(url.toString());
    return await res.json();
}