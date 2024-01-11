using Humanizer;

namespace K9_Koinz.Utils
{
    public static class DateUtils
    {
        public static DateTime StartOfWeek(this DateTime dt)
        {
            return dt.AtMidnight().AddDays(-(int)dt.DayOfWeek);
        }

        public static DateTime EndOfWeek(this DateTime dt)
        {
            return dt.StartOfWeek().AtMidnight().AddDays(7).AddSeconds(-1);
        }

        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        }

        public static DateTime StartOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1).AtMidnight();
        }

        public static DateTime EndOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year + 1, 1, 1).AddDays(-1).AtMidnight();
        }

        public static double GetPercentThroughWeek(this DateTime dt)
        {
            var startDate = dt.StartOfWeek();
            var today = dt.AtMidnight();
            return (today - startDate).TotalDays * 100 / 7;
        }

        public static double GetPercentThroughMonth(this DateTime dt)
        {
            var startDate = dt.StartOfMonth();
            var endDate = dt.EndOfMonth();
            var today = dt.AtMidnight();
            var totalDays = (endDate - startDate).Days;
            return (today - startDate).Days * 100 / totalDays;
        }
        public static double GetPercentThroughYear(this DateTime dt)
        {
            var startDate = dt.StartOfYear();
            var endDate = dt.EndOfYear();
            var today = dt.AtMidnight();
            var totalDays = (endDate - startDate).Days;
            return (today - startDate).Days * 100 / totalDays;
        }

        public static string FormatForUrl(this DateTime dt)
        {
            return dt.Year.ToString().PadLeft(4, '0') + "-" +
                dt.Month.ToString().PadLeft(2, '0') + "-" +
                dt.Day.ToString().PadLeft(2, '0');
        }
    }
}
