using Humanizer;
using K9_Koinz.Models;

namespace K9_Koinz.Utils {
    public static class DateUtils {
        private static readonly string[] MONTH_NAMES = {
            "January",
            "Februrary",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

        public static DateTime StartOfWeek(this DateTime dt) {
            return dt.AtMidnight().AddDays(-(int)dt.DayOfWeek);
        }

        public static DateTime EndOfWeek(this DateTime dt) {
            return dt.StartOfWeek().AtMidnight().AddDays(7).AddSeconds(-1);
        }

        public static DateTime StartOfMonth(this DateTime dt) {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime dt) {
            return new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddSeconds(-1);
        }

        public static DateTime StartOfYear(this DateTime dt) {
            return new DateTime(dt.Year, 1, 1).AtMidnight();
        }

        public static DateTime EndOfYear(this DateTime dt) {
            return new DateTime(dt.Year + 1, 1, 1).AddDays(-1).AtMidnight();
        }

        public static DateTime GetPreviousPeriod(this DateTime dt, BudgetTimeSpan timespan) {
            if (timespan == BudgetTimeSpan.WEEKLY) {
                return dt.AddDays(-7);
            }
            if (timespan == BudgetTimeSpan.MONTHLY) {
                return dt.AddMonths(-1);
            }
            if (timespan == BudgetTimeSpan.YEARLY) {
                return dt.AddYears(-1);
            }
            return dt;
        }

        public static DateTime GetNextPeriod(this DateTime dt, BudgetTimeSpan timespan) {
            if (timespan == BudgetTimeSpan.WEEKLY) {
                return dt.AddDays(7);
            }
            if (timespan == BudgetTimeSpan.MONTHLY) {
                return dt.AddMonths(1);
            }
            if (timespan == BudgetTimeSpan.YEARLY) {
                return dt.AddYears(1);
            }
            return dt;
        }

        public static double GetPercentThroughWeek(this DateTime dt) {
            var startDate = dt.StartOfWeek();
            return (dt - startDate).TotalDays * 100 / 7;
        }

        public static double GetPercentThroughMonth(this DateTime dt) {
            var startDate = dt.StartOfMonth();
            var endDate = dt.EndOfMonth();
            var totalDays = (endDate - startDate).Days;
            return (dt - startDate).Days * 100 / totalDays;
        }
        public static double GetPercentThroughYear(this DateTime dt) {
            var startDate = dt.StartOfYear();
            var endDate = dt.EndOfYear();
            var totalDays = (endDate - startDate).Days;
            return (dt - startDate).Days * 100 / totalDays;
        }

        public static string FormatForUrl(this DateTime dt) {
            return dt.Year.ToString().PadLeft(4, '0') + "-" +
                dt.Month.ToString().PadLeft(2, '0') + "-" +
                dt.Day.ToString().PadLeft(2, '0');
        }

        public static string GetMonthName(this DateTime dt) {
            return MONTH_NAMES[dt.Month - 1];
        }

        public static string GetMonthName(int month) {
            return MONTH_NAMES[month - 1];
        }

        public static string FormatShortMonthAndYear(this DateTime dt) {
            return dt.GetMonthName().Substring(0, 3) + " '" + dt.Year.ToString().Substring(2, 2);
        }

        public static string FormatShortString(this DateTime dt) {
            return dt.GetMonthName().Substring(0, 3) + " " + dt.Day + ", " + dt.Year;
        }
    }
}
