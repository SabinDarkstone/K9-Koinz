using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9_Koinz.Utils;

namespace K9_Koinz.Models
{
    public enum BudgetTimeSpan {
        [Display(Name = "Weekly")]
        WEEKLY,
        [Display(Name = "Monthly")]
        MONTHLY,
        [Display(Name = "Yearly")]
        YEARLY
    }

    public class Budget : INameable {
        public Guid Id { get; set; }
        [Unique<Budget>]
        public string Name { get; set; } = "New Budget";
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public BudgetTimeSpan Timespan { get; set; }

        public ICollection<BudgetLine> BudgetLines { get; set; } = new List<BudgetLine>();

        [NotMapped]
        public ICollection<BudgetLine> UnallocatedLines { get; set; } = new List<BudgetLine>();

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		[NotMapped]
        public ICollection<BudgetLine> IncomeLines {
            get {
                return BudgetLines.Where(line => line.LineType == BudgetLineType.INCOME).ToList();
            }
        }

        [NotMapped]
        public ICollection<BudgetLine> ExpenseLines {
            get {
                return BudgetLines.Where(line => line.LineType == BudgetLineType.EXPENSE).ToList();
            }
        }

        [NotMapped]
        public string TimespanString {
            get {
                return this.Timespan.GetAttribute<DisplayAttribute>().Name;
            }
        }
    }
}
