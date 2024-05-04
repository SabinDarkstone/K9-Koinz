using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models {

    public enum RepeatMode {
        [Display(Name = "On a Specific Day")]
        SPECIFIC_DAY = 0,
        [Display(Name = "Every X")]
        INTERVAL = 1
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

    public class RepeatConfig : BaseEntity {
        public RepeatMode Mode { get; set; }

        [DisplayName("Repeat Frequency")]
        public RepeatFrequency Frequency { get; set; }

        [DisplayName("Repeat Gap")]
        public int? IntervalGap { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FirstFiring { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? TerminationDate { get; set; }

        [DisplayName("Previous Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PreviousFiring { get; set; }

        [DisplayName("Next Firing")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime? NextFiring { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime? CalculatedNextFiring {
            get {
                if (NextFiring == null) {
                    NextFiring = GetNextFireDate();
                }

                return NextFiring;
            }
        }

        public bool IsActive {
            get {
                if (TerminationDate.HasValue) {
                    return TerminationDate.Value.Date > DateTime.Today;
                } else {
                    return true;
                }
            }
        }

        public bool NeedsToFireImmediately {
            get {
                return FirstFiring <= DateTime.Now && PreviousFiring == null;
            }
        }

        public string RepeatString => this.GetRepeatString();

        public void FireNow() {
            PreviousFiring = CalculatedNextFiring;
            NextFiring = GetNextFireDate();
        }

        private DateTime? GetNextFireDate() {
            DateTime proposedDate;
            // If this has never fired yet, the next firing is the same
            // as the first firing
            if (PreviousFiring == null) {
                return FirstFiring;
            }

            if (Mode == RepeatMode.SPECIFIC_DAY) {
                proposedDate = CalculateNextSpecificFiring();
            } else if (Mode == RepeatMode.INTERVAL) {
                proposedDate = CalculateNextIntervalFiring();
            } else {
                throw new Exception("Either no repeat mode or an invalid repeat mode was specified");
            }

            if (TerminationDate.HasValue && proposedDate > TerminationDate.Value) {
                return null;
            }

            return proposedDate;
        }

        private DateTime CalculateNextSpecificFiring() {
            // Get either the last firing, or if that is null, the first firing
            DateTime lastOrFirstFiring = PreviousFiring ?? FirstFiring;

            return Frequency switch {
                RepeatFrequency.DAILY => lastOrFirstFiring.AddDays(1),
                RepeatFrequency.WEEKLY => lastOrFirstFiring.AddDays(7),
                RepeatFrequency.MONTHLY => lastOrFirstFiring.AddMonths(1),
                RepeatFrequency.YEARLY => lastOrFirstFiring.AddYears(1),
                _ => throw new Exception("Unknown frequency value chosen"),
            };
        }

        private DateTime CalculateNextIntervalFiring() {
            // Get either the last firing, or if that is null, the first firing
            DateTime lastOrFirstFiring = PreviousFiring ?? FirstFiring;

            if (!IntervalGap.HasValue) {
                throw new Exception("Interval gap cannot be null in internal mode.");
            }

            return Frequency switch {
                RepeatFrequency.DAILY => lastOrFirstFiring.AddDays(IntervalGap.Value),
                RepeatFrequency.WEEKLY => lastOrFirstFiring.AddDays(7 * IntervalGap.Value),
                RepeatFrequency.MONTHLY => lastOrFirstFiring.AddMonths(IntervalGap.Value),
                RepeatFrequency.YEARLY => lastOrFirstFiring.AddYears(IntervalGap.Value),
                _ => throw new Exception("Unknown frequency value chosen")
            };
        }
    }
}
