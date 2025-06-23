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
    }
}
