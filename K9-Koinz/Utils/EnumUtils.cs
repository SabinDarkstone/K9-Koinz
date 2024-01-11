using K9_Koinz.Models;
using System;
using System.Reflection;

namespace K9_Koinz.Utils
{
    public static class EnumUtils
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue) where TAttribute : Attribute
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<TAttribute>();
        }

        public static (DateTime, DateTime) GetStartAndEndDate(this BudgetTimeSpan timespan)
        {
            DateTime startDate, endDate;

            switch (timespan)
            {
                case BudgetTimeSpan.WEEKLY:
                    startDate = DateTime.Now.StartOfWeek();
                    endDate = DateTime.Now.EndOfWeek();
                    break;
                default:
                case BudgetTimeSpan.MONTHLY:
                    startDate = DateTime.Now.StartOfMonth();
                    endDate = DateTime.Now.EndOfMonth();
                    break;
                case BudgetTimeSpan.YEARLY:
                    startDate = DateTime.Now.StartOfYear();
                    endDate = DateTime.Now.EndOfYear();
                    break;
            }

            return (startDate, endDate);
        }
    }
}
