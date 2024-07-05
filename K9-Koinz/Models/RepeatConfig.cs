using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models {

    [Obsolete]
    public enum RepeatMode {
        [Display(Name = "On a Specific Day")]
        SPECIFIC_DAY = 0,
        [Display(Name = "Every X")]
        INTERVAL = 1
    }

    [Obsolete]
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

    [Obsolete]
    public class RepeatConfig : BaseEntity {
        [Obsolete]
        public bool DoRepeat { get; set; }
        
        [Obsolete]
        public RepeatMode Mode { get; set; }

        [Obsolete]
        [DisplayName("Repeat Frequency")]
        public RepeatFrequency Frequency { get; set; }

        [Obsolete]
        [DisplayName("Repeat Gap")]
        public int? IntervalGap { get; set; }

        [Obsolete]
        [DisplayName("Start Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FirstFiring { get; set; }

        [Obsolete]
        [DisplayName("End Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? TerminationDate { get; set; }

        [Obsolete]
        [DisplayName("Previous Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PreviousFiring { get; set; }

        [Obsolete]
        [DisplayName("Next Firing")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime? NextFiring { get; set; }

        [Obsolete]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime? CalculatedNextFiring {
            get {
                if (!IsActive) {
                    return null;
                }

                if (NextFiring == null) {
                    NextFiring = GetNextFireDate();
                }

                return NextFiring;
            }
        }

        [Obsolete]
        public bool IsActive {
            get {
                if (!DoRepeat && FirstFiring == PreviousFiring) {
                    return false;
                }

                if (TerminationDate.HasValue) {
                    return TerminationDate.Value.Date > DateTime.Today;
                } else {
                    return true;
                }
            }
        }

        [Obsolete]
        public bool NeedsToFireImmediately {
            get {
                return FirstFiring <= DateTime.Now && PreviousFiring == null;
            }
        }

        [Obsolete]
        public string RepeatString => this.GetRepeatString();

        [Obsolete]
        public void FireNow() {
            PreviousFiring = CalculatedNextFiring;
            NextFiring = GetNextFireDate();
        }

        [Obsolete]
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

        [Obsolete]
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

        [Obsolete]
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
