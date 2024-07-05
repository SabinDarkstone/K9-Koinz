using K9_Koinz.Models.Enums;

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
            return dt.Date.AddDays(-(int)dt.DayOfWeek);
        }

        public static DateTime EndOfWeek(this DateTime dt) {
            return dt.StartOfWeek().Date.AddDays(7).AddSeconds(-1);
        }

        public static DateTime StartOfMonth(this DateTime dt) {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime dt) {
            return new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddSeconds(-1);
        }

        public static DateTime StartOfYear(this DateTime dt) {
            return new DateTime(dt.Year, 1, 1).Date;
        }

        public static DateTime EndOfYear(this DateTime dt) {
            return new DateTime(dt.Year + 1, 1, 1).AddDays(-1).Date;
        }

        public static DateTime GetPreviousPeriod(this DateTime dt, RecurrenceType timespan) {
            if (timespan == RecurrenceType.WEEKLY) {
                return dt.AddDays(-7);
            }
            if (timespan == RecurrenceType.MONTHLY) {
                return dt.AddMonths(-1);
            }
            if (timespan == RecurrenceType.YEARLY) {
                return dt.AddYears(-1);
            }
            return dt;
        }

        public static DateTime GetNextPeriod(this DateTime dt, RecurrenceType timespan) {
            if (timespan == RecurrenceType.WEEKLY) {
                return dt.AddDays(7);
            }
            if (timespan == RecurrenceType.MONTHLY) {
                return dt.AddMonths(1);
            }
            if (timespan == RecurrenceType.YEARLY) {
                return dt.AddYears(1);
            }
            return dt;
        }

        public static double GetPercentThroughWeek(this DateTime dt) {
            var startDate = dt.StartOfWeek();
            return (dt.Day - startDate.Day + 1) / 7d * 100;
        }

        public static double GetPercentThroughMonth(this DateTime dt) {
            var totalDays = DateTime.DaysInMonth(dt.Year, dt.Month);
            return (double)dt.Day / totalDays * 100;
        }
        public static double GetPercentThroughYear(this DateTime dt) {
            return dt.DayOfYear / 365.25 * 100;
        }

        public static string FormatForUrl(this DateTime? dt) {
            if (dt == null) {
                return string.Empty;
            } else {
                return FormatForUrl(dt.Value);
            }
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

        public static int GetMonthNumber(string monthName) {
            return MONTH_NAMES.Select(m => m.ToLower()).ToList().IndexOf(monthName.ToLower()) + 1;
        }

        public static string FormatShortMonthAndYear(this DateTime dt) {
            return dt.GetMonthName().Substring(0, 3) + " '" + dt.Year.ToString().Substring(2, 2);
        }

        public static string FormatShortString(this DateTime dt) {
            return dt.GetMonthName().Substring(0, 3) + " " + dt.Day + ", " + dt.Year;
        }
    }
}
