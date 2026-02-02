namespace Hpd.Api.Helpers
{
    public static class TimeRangeParser
    {
        public static TimeSpan Parse(string value, TimeSpan fallback)
        {
            if (string.IsNullOrWhiteSpace(value)) return fallback;

            value = value.Trim().ToLowerInvariant();

            // examples: 24h, 7d, 30m
            var numberPart = new string(value.TakeWhile(char.IsDigit).ToArray());
            var unitPart = new string(value.SkipWhile(char.IsDigit).ToArray());

            if (!int.TryParse(numberPart, out var n) || n <= 0) return fallback;

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
