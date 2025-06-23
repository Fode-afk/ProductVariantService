using System.Globalization;

namespace ProductService.Utils
{
    public static class DateTimeUtil
    {
        public static DateTimeOffset GetCurrentTimeFormatted(IConfiguration config)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(config["Time"]);
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            return new DateTimeOffset(localDateTime, timeZone.GetUtcOffset(localDateTime));
        }

        public static DateTimeOffset? ParseDate(string date)
        {
            DateTimeOffset res;
            if (DateTimeOffset.TryParse(date,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
            {
                return res;
            }
            return null;
        }
    }
}
