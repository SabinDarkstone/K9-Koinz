using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9_Koinz.Utils;

namespace K9_Koinz.Models {
    public class BudgetLine : DateTrackedEntity {
        [DisplayName("Category")]
        public Guid BudgetCategoryId { get; set; }
        public Category BudgetCategory { get; set; }
        [DisplayName("Budget Name")]
        public Guid BudgetId { get; set; }
        public Budget Budget {  get; set; }
        [DisplayName("Budgeted Amount")]
        public double BudgetedAmount { get; set; }

        [NotMapped]
        public bool IsTopLevelCategory {
            get {
                if (BudgetCategory == null) return false;
                return BudgetCategory.ParentCategoryId == null;
            }
        }

        [NotMapped]
        public double SpentAmount { get; set; }

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
        public double SpentPercent {
            get {
                if (BudgetedAmount == 0) return 0;
                return Math.Clamp(Math.Floor((double)(SpentAmount / BudgetedAmount) * 100), 0, 100);
            }
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
