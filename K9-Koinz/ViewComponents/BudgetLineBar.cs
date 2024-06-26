﻿using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.ViewComponents {

    [ViewComponent(Name = "BudgetLineBar")]
    public class BudgetLineBar : KoinzController<BudgetLineBar> {
        private BudgetLine line;

        public BudgetLineBar(KoinzContext context, ILogger<BudgetLineBar> logger)
            : base(context, logger) { }

        public DateTime CurrentPeriod { get; set; }

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

        public string CategoryIcon {
            get {
                return line.BudgetCategory.FontAwesomeIcon;
            }
        }

        public bool RolloverInactive {
            get {
                return line.RolloverStatus == RolloverStatus.NOT_READY || line.RolloverStatus == RolloverStatus.NONE;
            }
        }

        public bool HadExtraMoneyLastPeriod {
            get {
                return line.RolloverStatus == RolloverStatus.POSITIVE || line.RolloverStatus == RolloverStatus.ZERO;
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

        public string WeeklyAmount {
            get {
                if (line.ShowWeeklyLines) {
                    var monthlyAmount = line.BudgetedAmount;

                    if (WentOverBudgetLastPeriod) {
                        monthlyAmount -= Math.Abs(line.RolloverAmount.Value);
                    } else if (HadExtraMoneyLastPeriod) {
                        monthlyAmount += Math.Abs(line.RolloverAmount.Value);
                    }

                    var weeklyAmount = (monthlyAmount * 12) / 52;
                    return weeklyAmount.FormatCurrency(0) + " Per Week";
                }

                return null;
            }
        }

        public List<double> SundayPercentLines {
            get {
                if (!line.ShowWeeklyLines) {
                    return new List<double>();
                }

                var dates = new List<double>();
                var daysInMonth = DateTime.DaysInMonth(CurrentPeriod.Year, CurrentPeriod.Month);
                for (int day = 1; day <= daysInMonth; day++) {
                    var date = new DateTime(CurrentPeriod.Year, CurrentPeriod.Month, day);
                    if (date.DayOfWeek == DayOfWeek.Sunday) {
                        dates.Add(100 * (float)day / daysInMonth);
                    }
                }

                return dates;
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
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
                var overagePercent = Math.Clamp(Math.Abs(line.RolloverAmount.Value) / line.BudgetedAmount * 100, 0, 100);

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

            if (line.BudgetCategory.CategoryType == CategoryType.INCOME) {
                SolidProgressBarClassList = "progress-bar bg-primary";
                StripedProgressBarClassList = "progress-bar progress-bar-striped bg-primary";
            }

            return View(this);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
