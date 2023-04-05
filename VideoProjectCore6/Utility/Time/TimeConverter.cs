using VideoProjectCore6.Utility.Time;

namespace VideoProjectCore6.Utilities.Time
{
    public static class TimeConverter
    {
        public static DateTime ConvertFromUtc(DateTime dt, string tzId)
        {
            var ianaMapper = new IanaWinMapper();

            var winTimeZone = ianaMapper.IanaTimeZoneToWinTimeZone(tzId);

            if(winTimeZone != null)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.FindSystemTimeZoneById(winTimeZone));
            }
            return dt;
        }
    }
}
