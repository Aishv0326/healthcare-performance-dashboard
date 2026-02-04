namespace Hpd.Api.Helpers
{
    /// <summary>
    /// Utility helper to parse simple duration strings used by API query parameters.
    ///
    /// Supported formats:
    /// - "30m" => 30 minutes
    /// - "24h" => 24 hours
    /// - "7d"  => 7 days
    ///
    /// If parsing fails, the method returns a safe fallback value.
    /// This prevents invalid user input from breaking the API.
    /// </summary>
    public static class TimeRangeParser
    {
        /// <summary>
        /// Parses a duration string like "24h" or "5m" into a TimeSpan.
        /// </summary>
        /// <param name="value">Input duration string (e.g., "24h", "30m", "7d").</param>
        /// <param name="fallback">Value returned if parsing fails or input is invalid.</param>
        public static TimeSpan Parse(string value, TimeSpan fallback)
        {
            if (string.IsNullOrWhiteSpace(value)) return fallback;

            value = value.Trim().ToLowerInvariant();

            // Split numeric part + unit part
            // Example: "24h" -> numberPart="24", unitPart="h"
            var numberPart = new string(value.TakeWhile(char.IsDigit).ToArray());
            var unitPart = new string(value.SkipWhile(char.IsDigit).ToArray());

            // Validate numeric portion
            if (!int.TryParse(numberPart, out var n) || n <= 0) return fallback;

            // Convert supported units
            return unitPart switch
            {
                "m" => TimeSpan.FromMinutes(n),
                "h" => TimeSpan.FromHours(n),
                "d" => TimeSpan.FromDays(n),
                _ => fallback
            };
        }
    }
}
