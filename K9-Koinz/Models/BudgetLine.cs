using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;

namespace K9_Koinz.Models {
    public enum RolloverStatus {
        NONE,
        POSITIVE,
        NEGATIVE,
        NOT_READY
    }

    public class BudgetLine : BaseEntity {
        [DisplayName("Category")]
        public Guid BudgetCategoryId { get; set; }

        public Category BudgetCategory { get; set; }

        public string BudgetCategoryName { get; set; }

        [DisplayName("Budget Name")]
        public Guid BudgetId { get; set; }

        public Budget Budget { get; set; }

        public string BudgetName { get; set; }

        [DisplayName("Budgeted Amount")]
        [Column(TypeName = "decimal(10, 2)")]
        public double BudgetedAmount { get; set; }

        [DisplayName("Rollover Unspent Money")]
        public bool DoRollover { get; set; }
        [DisplayName("Always Green")]
        public bool GreenBarAlways { get; set; }
        [DisplayName("Show Week Dividers in Bar")]
        public bool ShowWeeklyLines { get; set; }

        public List<BudgetLinePeriod> Periods { get; set; } = new();


        [NotMapped]
        public bool IsTopLevelCategory {
            get {
                if (BudgetCategory == null) return false;
                return BudgetCategory.ParentCategoryId == null;
            }
        }

        [NotMapped]
        public BudgetLinePeriod CurrentPeriod { get; set; }
        [NotMapped]
        public BudgetLinePeriod PreviousPeriod { get; private set; }

        public void GetCurrentAndPreviousPeriods(DateTime refDate) {
            if (Periods == null) {
                throw new Exception("Attempt to query budget line periods without retrieving them from the database.  Please make sure to Include BudgetLinePeriods in your query.");
            }
            if (Budget == null) {
                throw new Exception("Attempt to query budget without retrieving it from the database.  Please make sure to Include Budget in your query.");
            }

            var prevRefDate = refDate.GetPreviousPeriod(Budget.Timespan);

            CurrentPeriod = Periods.SingleOrDefault(per => per.StartDate <= refDate && per.EndDate >= refDate);
            PreviousPeriod = Periods.SingleOrDefault(per => per.StartDate <= prevRefDate && per.EndDate >= prevRefDate);
        }

        [NotMapped]
        public double SpentAmount { get; set; }

        [NotMapped]
        public double SpentAndNegRolloverAmount { get; set; }

        [NotMapped]
        public RolloverStatus RolloverStatus {
            get {
                if (!DoRollover || CurrentPeriod == null) {
                    return RolloverStatus.NONE;
                }

                if (PreviousPeriod == null && CurrentPeriod != null) {
                    return RolloverStatus.NOT_READY;
                }

                if (CurrentPeriod.StartingAmount < 0) {
                    return RolloverStatus.NEGATIVE;
                } else {
                    return RolloverStatus.POSITIVE;
                }
            }
        }

        [NotMapped]
        public List<Transaction> Transactions { get; set; }

        [NotMapped]
        public double TimePerent {
            get {
                if (Budget == null) return 0;

                if (Budget.Timespan == BudgetTimeSpan.WEEKLY) {
                    return DateTime.Now.GetPercentThroughWeek();
                } else if (Budget.Timespan == BudgetTimeSpan.MONTHLY) {
                    return DateTime.Now.GetPercentThroughMonth();
                } else if (Budget.Timespan == BudgetTimeSpan.YEARLY) {
                    return DateTime.Now.GetPercentThroughYear();
                } else {
                    return 0;
                }
            }
        }

        [NotMapped]
        public double? RolloverAmount {
            get {
                if (DoRollover && CurrentPeriod != null) {
                    return CurrentPeriod.StartingAmount;
                }
                return null;
            }
        }
    }
}
