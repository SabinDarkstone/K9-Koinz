using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.ViewComponents {
    [ViewComponent(Name = "BudgetLineBar")]
    public class BudgetLineBar : ViewComponent {
        private BudgetLine line;

        public  DateTime CurrentPeriod { get; set; }

        public string TodayLineStyle {
            get {
                return "left: " + line.TimePerent + "%;";
            }
        }

        public DateTime StartDate {
            get {
                return line.Budget.Timespan.GetStartAndEndDate(CurrentPeriod).Item1;
            }
        }

        public DateTime EndDate {
            get {
                return line.Budget.Timespan.GetStartAndEndDate(CurrentPeriod).Item2;
            }
        }

        public string StartDateString {
            get {
                return StartDate.FormatForUrl();
            }
        }

        public string EndDateString {
            get {
                return EndDate.FormatForUrl();
            }
        }

        public string TimespanString {
            get {
                return line.Budget.TimespanString;
            }
        }

        public Guid CategoryId {
            get {
                return line.BudgetCategoryId;
            }
        }

        public string CategoryName {
            get {
                return line.BudgetCategory.Name;
            }
        }

        public bool RolloverInactive {
            get {
                return line.RolloverStatus == RolloverStatus.NOT_READY || line.RolloverStatus == RolloverStatus.NONE;
            }
        }

        public bool HadExtraMoneyLastPeriod {
            get {
                return line.RolloverStatus == RolloverStatus.POSITIVE;
            }
        }

        public bool WentOverBudgetLastPeriod {
            get {
                return line.RolloverStatus == RolloverStatus.NEGATIVE;
            }
        }

        public string SpentAmountString {
            get {
                return line.SpentAmount.FormatCurrency(0);
            }
        }

        public string BudgetedAmountString {
            get {
                return line.BudgetedAmount.FormatCurrency(0);
            }
        }

        public bool ShowOverUnderAmount {
            get {
                return line.DoRollover && line.CurrentPeriod != null;
            }
        }

        public string RolloverAmountString {
            get {
                return Math.Abs(line.RolloverAmount.Value).FormatCurrency(0);
            }
        }

        public bool ShowAmountLeft {
            get {
                return !line.DoRollover || (line.DoRollover && line.PreviousPeriod == null);
            }
        }

        public bool HasExtraNow {
            get {
                return BudgetedAmount >= SpentAmount;
            }
        }

        public bool ShowAmountLeftWithRollover {
            get {
                return line.DoRollover && line.CurrentPeriod != null && line.PreviousPeriod != null;
            }
        }

        public double StartingAmount {
            get {
                if (line.CurrentPeriod == null) {
                    return 0;
                } else {
                    return line.CurrentPeriod.StartingAmount;
                }
            }
        }

        public double BudgetedAmount => line.BudgetedAmount;
        public double SpentAmount => line.SpentAmount;

        public string SolidWidthString { get; set; }
        public string StripedWidthString { get; set; }

        public string SolidProgressBarClassList { get; set; }
        public string StripedProgressBarClassList { get; set; }

        public double SpentOverBudgetedPercent { get; set; }

        public double TotalBarPercent { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(BudgetLine line, DateTime currentPeriod) {
            this.line = line;
            this.CurrentPeriod = currentPeriod;

            SolidProgressBarClassList = "progress-bar";
            StripedProgressBarClassList = "progress-bar progress-bar-striped";

            SpentOverBudgetedPercent = Math.Clamp((Math.Abs(line.SpentAmount) / line.BudgetedAmount) * 100, 0, 100);

            if (line.RolloverStatus == RolloverStatus.NONE) {
                SolidWidthString = "width: " + SpentOverBudgetedPercent + "%;";
                TotalBarPercent = SpentOverBudgetedPercent;
            } else if (line.RolloverStatus == RolloverStatus.POSITIVE) {
                var spentOverBudgetedPlusRolloverPercent = Math.Clamp((Math.Abs(line.SpentAmount) / (line.BudgetedAmount + line.RolloverAmount.Value)) * 100, 0, 100);
                var remainingPercent = SpentOverBudgetedPercent - spentOverBudgetedPlusRolloverPercent;

                SolidWidthString = "width: " + remainingPercent + "%;";
                StripedWidthString = "width: " + spentOverBudgetedPlusRolloverPercent + "%;";
                TotalBarPercent = SpentOverBudgetedPercent;

                if (spentOverBudgetedPlusRolloverPercent >= 99.5 && !line.GreenBarAlways) {
                    StripedProgressBarClassList += " bg-danger";
                } else if (spentOverBudgetedPlusRolloverPercent >= 80 && !line.GreenBarAlways) {
                    StripedProgressBarClassList += " bg-warning";
                } else {
                    StripedProgressBarClassList += " bg-success";
                }
            } else {
                var overagePercent = Math.Clamp(Math.Abs(line.RolloverAmount.Value) / line.BudgetedAmount, 0, 100);

                SolidWidthString = "width: " + SpentOverBudgetedPercent + "%;";
                StripedWidthString = "width: " + overagePercent + "%;";
                TotalBarPercent = overagePercent + SpentOverBudgetedPercent;

                if (TotalBarPercent >= 99.5 && !line.GreenBarAlways) {
                    StripedProgressBarClassList += " bg-danger";
                } else if (TotalBarPercent >= 80 && !line.GreenBarAlways) {
                    StripedProgressBarClassList += " bg-warning";
                } else {
                    StripedProgressBarClassList += " bg-success";
                }
            }

            if (TotalBarPercent >= 99.5 && !line.GreenBarAlways) {
                SolidProgressBarClassList += " bg-danger";
            } else if (TotalBarPercent >= 80 && !line.GreenBarAlways) {
                SolidProgressBarClassList += " bg-warning";
            } else {
                SolidProgressBarClassList += " bg-success";
            }

            return View(this);
        }
    }
}
