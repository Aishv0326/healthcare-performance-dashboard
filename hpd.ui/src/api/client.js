const BASE_URL = "https://localhost:7011";

export async function getSummary(range = "24h") {
    const res = await fetch(`${BASE_URL}/api/Metrics/summary?range=${range}`);
    return await res.json();
}

export async function getTrends(range = "24h", bucket = "5m") {
    const res = await fetch(
        `${BASE_URL}/api/Metrics/trends?range=${range}&bucket=${bucket}`
    );
    return await res.json();
}

export async function getTopEndpoints(range = "24h") {
    const res = await fetch(
        `${BASE_URL}/api/Metrics/top-endpoints?range=${range}`
    );
    return await res.json();
}
