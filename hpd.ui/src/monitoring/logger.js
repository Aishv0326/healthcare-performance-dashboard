// Key used to store telemetry data in browser localStorage.
// This allows us to persist client-side events across page reloads.
const STORAGE_KEY = "hpd_telemetry";

/**
 * Safely read telemetry data from localStorage.
 * - Returns an empty array if data is missing or corrupted.
 * - Prevents the UI from crashing due to malformed JSON.
 */
function safeRead() {
    try {
        return JSON.parse(localStorage.getItem(STORAGE_KEY) || "[]");
    } catch {
        return [];
    }
}

/**
 * Safely write telemetry data to localStorage.
 * - Keeps only the most recent 500 events to avoid unbounded growth.
 * - Silently ignores storage quota or browser errors.
 */
function safeWrite(items) {
    try {
        localStorage.setItem(
            STORAGE_KEY,
            JSON.stringify(items.slice(-500)) // retain last 500 events
        );
    } catch {
        // Intentionally ignored:
        // Telemetry must never break the main application flow
    }
}

/**
 * Track a custom client-side event.
 * Examples:
 * - Dashboard loaded
 * - KPI card clicked
 * - Filter or range changed
 *
 * @param {string} name - Event name
 * @param {object} data - Optional contextual metadata
 */
export function trackEvent(name, data = {}) {
    const entry = {
        type: "event",
        name,
        data,
        ts: new Date().toISOString(),
    };

    // Log to console for developer visibility
    console.log("📈 EVENT:", entry);

    const items = safeRead();
    items.push(entry);
    safeWrite(items);
}

/**
 * Track a client-side error.
 * Used for:
 * - Runtime exceptions
 * - Failed async operations
 * - Unexpected UI crashes
 *
 * @param {Error|string} error - Error object or message
 * @param {object} context - Optional contextual information
 */
export function trackError(error, context = {}) {
    const entry = {
        type: "error",
        message: error?.message || String(error),
        stack: error?.stack,
        context,
        ts: new Date().toISOString(),
    };

    // Log to console to surface issues during development
    console.error("🧯 ERROR:", entry);

    const items = safeRead();
    items.push(entry);
    safeWrite(items);
}

/**
 * Retrieve all stored telemetry events.
 * Useful for:
 * - Debugging
 * - Manual inspection during demos
 * - Future upload to a real monitoring backend
 */
export function getTelemetry() {
    return safeRead();
}

/**
 * Clear all stored telemetry data.
 * Typically used during development or demo resets.
 */
export function clearTelemetry() {
    safeWrite([]);
}
