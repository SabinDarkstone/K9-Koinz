using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models {

    public class RecurringEventException : Exception {
        public RecurringEventException(string message) : base(message) { }
    }

    public enum RepeatFrequency {
        [Display(Name = "Daily")]
        DAILY = 3,
        [Display(Name = "Weekly")]
        WEEKLY = 2,
        [Display(Name = "Monthly")]
        MONTHLY = 0,
        [Display(Name = "Yearly")]
        YEARLY = 1
    }

    public enum EndType {
        [Display(Name = "On a Day")]
        DATE = 0,
        [Display(Name = "After N Occurences")]
        COUNT = 1
    }

    public class RepeatConfig2 : BaseEntity {

        [Required]
        [DisplayName("Repeat Frequency")]
        public RepeatFrequency Frequency { get; set; }

        [DisplayName("Interval")]
        public int Interval { get; set; }

        [DisplayName("Day of the Week")]
        public DayOfWeek DayOfWeek { get; set; }

        [Range(1, 31)]
        [DisplayName("Day of the Month")]
        public int DayOfMonth { get; set; }

        [Range(1, 12)]
        [DisplayName("Month of the Year")]
        public int MonthOfYear { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [DisplayName("Previous Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PreviousDate { get; set; }

        public int CurrentRepeatCount { get; set; }

        [DisplayName("Number of Occurrences")]
        public int? CountBeforeEnd { get; set; }

        public Guid RecurringItemId { get; set; }

        [DisplayName("Next Date")]
        public DateTime? NextOccurrence {
            get {
                // The next occurrence will always be the first if no previous date exists
                if (PreviousDate == null) {
                    return StartDate;
                }

                // Check if enough occurrences have happened
                if (CountBeforeEnd != null && CurrentRepeatCount >= CountBeforeEnd) {
                    return null;
                }

                DateTime nextDate;

                if (Frequency == RepeatFrequency.DAILY) {
                    nextDate =  PreviousDate.Value.AddDays(Interval);
                } else if (Frequency == RepeatFrequency.WEEKLY) {
                    nextDate = PreviousDate.Value.AddDays(7 * Interval);
                } else if (Frequency == RepeatFrequency.MONTHLY) {
                    nextDate = PreviousDate.Value.AddMonths(Interval);
                    int daysInMonth = DateTime.DaysInMonth(nextDate.Year, nextDate.Month);
                    if (nextDate.Day < StartDate.Day && nextDate.Day < daysInMonth) {
                        nextDate = new DateTime(nextDate.Year, nextDate.Month, StartDate.Day);
                    }
                } else if (Frequency == RepeatFrequency.YEARLY) {
                    nextDate = PreviousDate.Value.AddYears(Interval);
                } else {
                    throw new RecurringEventException("Unknown repeat frequency: " + Frequency.GetAttribute<DisplayAttribute>().GetName());
                }

                // Check if the next date is after the end date
                if (EndDate != null && nextDate > EndDate) {
                    return null;
                }

                return nextDate;
            }
        }
    }
}
