using K9_Koinz.Models;
using System.Reflection;

namespace K9_Koinz.Utils {
    public static class EnumUtils {
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue) where TAttribute : Attribute {
            if (enumValue == null) {
                return null;
            }

            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<TAttribute>();
        }

        public static (DateTime, DateTime) GetStartAndEndDate(this BudgetTimeSpan timespan) {
            return timespan.GetStartAndEndDate(DateTime.Now);
        }

        public static (DateTime, DateTime) GetStartAndEndDate(this BudgetTimeSpan timespan, DateTime period) {
            DateTime startDate, endDate;

            switch (timespan) {
                case BudgetTimeSpan.WEEKLY:
                    startDate = period.StartOfWeek();
                    endDate = period.EndOfWeek();
                    break;
                default:
                case BudgetTimeSpan.MONTHLY:
                    startDate = period.StartOfMonth();
                    endDate = period.EndOfMonth();
                    break;
                case BudgetTimeSpan.YEARLY:
                    startDate = period.StartOfYear();
                    endDate = period.EndOfYear();
                    break;
            }

            return (startDate, endDate);
        }
    }
}
