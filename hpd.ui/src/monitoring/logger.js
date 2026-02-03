const STORAGE_KEY = "hpd_telemetry";

function safeRead() {
    try {
        return JSON.parse(localStorage.getItem(STORAGE_KEY) || "[]");
    } catch {
        return [];
    }
}

function safeWrite(items) {
    try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(items.slice(-500))); // keep last 500
    } catch {
        // ignore storage errors
    }
}

export function trackEvent(name, data = {}) {
    const entry = {
        type: "event",
        name,
        data,
        ts: new Date().toISOString(),
    };

    console.log("📈 EVENT:", entry);

    const items = safeRead();
    items.push(entry);
    safeWrite(items);
}

export function trackError(error, context = {}) {
    const entry = {
        type: "error",
        message: error?.message || String(error),
        stack: error?.stack,
        context,
        ts: new Date().toISOString(),
    };

    console.error("🧯 ERROR:", entry);

    const items = safeRead();
    items.push(entry);
    safeWrite(items);
}

export function getTelemetry() {
    return safeRead();
}

export function clearTelemetry() {
    safeWrite([]);
}
