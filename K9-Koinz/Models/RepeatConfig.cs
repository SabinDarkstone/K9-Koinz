using Humanizer;
using K9_Koinz.Models.Meta;
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
        public DateTime? LastFiring { get; set; }

        [DisplayName("Next Firing")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime? NextFiring {
            get {
                // If this has never fired yet, the next firing is the same
                // as the first firing
                if (LastFiring == null) {
                    return FirstFiring;
                }

                DateTime nextProposedFiring = DateTime.MinValue;
                if (Mode == RepeatMode.SPECIFIC_DAY) {
                    nextProposedFiring = CalculateNextSpecificFiring();
                } else if (Mode == RepeatMode.INTERVAL) {
                    nextProposedFiring = CalculateNextIntervalFiring();
                } else {
                    throw new Exception("Either no repeat mode or an invalid repeat mode was specified");
                }

                if (TerminationDate.HasValue && nextProposedFiring > TerminationDate.Value) {
                    return null;
                }

                return nextProposedFiring;
            }
        }

        public bool IsActive {
            get {
                return NextFiring.HasValue;
            }
        }

        public bool NeedsToFireImmediately {
            get {
                return FirstFiring <= DateTime.Now && LastFiring == null;
            }
        }

        public string RepeatString {
            get {
                if (!NextFiring.HasValue) {
                    return "Never - Expired";
                }

                if (Mode == RepeatMode.SPECIFIC_DAY) {
                    switch (Frequency) {
                        case RepeatFrequency.DAILY:
                            return "Every Day";
                        case RepeatFrequency.WEEKLY:
                            return "Every Week on " + NextFiring.Value.DayOfWeek.ToString();
                        case RepeatFrequency.MONTHLY:
                            return "Every Month on the " + NextFiring.Value.Date.Day.Ordinalize();
                        case RepeatFrequency.YEARLY:
                            return "Every Year on " + NextFiring.Value.Month + "/" + NextFiring.Value.Day;
                        default:
                            throw new Exception("Unknown repeat frequency chosen.");
                    }
                } else if (Mode == RepeatMode.INTERVAL) {
                    switch (Frequency) {
                        case RepeatFrequency.DAILY:
                            if (IntervalGap == 1) {
                                return "Every Day";
                            } else {
                                return "Every " + IntervalGap + " Days";
                            }
                        case RepeatFrequency.WEEKLY:
                            if (IntervalGap == 1) {
                                return "Every Week";
                            } else {
                                return "Every " + IntervalGap + " Weeks";
                            }
                        case RepeatFrequency.MONTHLY:
                            if (IntervalGap == 1) {
                                return "Every Month";
                            } else {
                                return "Every " + IntervalGap + " Months";
                            }
                        case RepeatFrequency.YEARLY:
                            if (IntervalGap == 1) {
                                return "Every Year";
                            } else {
                                return "Every " + IntervalGap + " Years";
                            }
                        default:
                            throw new Exception("Unknown repeat frequency chosen.");
                    }
                } else {
                    throw new Exception("Unknown repeat mode chosen");
                }
            }
        }

        public void FireNow() {
            LastFiring = NextFiring;
        }

        private DateTime CalculateNextSpecificFiring() {
            // Get either the last firing, or if that is null, the first firing
            DateTime lastOrFirstFiring = LastFiring ?? FirstFiring;

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
            DateTime lastOrFirstFiring = LastFiring ?? FirstFiring;

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
