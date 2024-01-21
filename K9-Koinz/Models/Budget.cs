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

    public class Budget : DateTrackedEntity, INameable {
        [Unique<Budget>]
        public string Name { get; set; } = "New Budget";
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public BudgetTimeSpan Timespan { get; set; }

        public virtual ICollection<BudgetLine> BudgetLines { get; set; } = new List<BudgetLine>();

        [NotMapped]
        public virtual ICollection<BudgetLine> UnallocatedLines { get; set; } = new List<BudgetLine>();

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		[NotMapped]
        public ICollection<BudgetLine> IncomeLines {
            get {
                return BudgetLines
                    .Where(line => line.BudgetCategory.CategoryType == CategoryType.INCOME)
                    .OrderBy(line => line.BudgetCategory.ParentCategory?.Name ?? "")
                        .ThenBy(line => line.BudgetCategory.Name)
                    .ToList();
            }
        }

        [NotMapped]
        public ICollection<BudgetLine> ExpenseLines {
            get {
                return BudgetLines
                    .Where(line => line.BudgetCategory.CategoryType == CategoryType.EXPENSE)
                    .OrderBy(line => line.BudgetCategory.ParentCategory?.Name ?? "")
                        .ThenBy(line => line.BudgetCategory.Name)
                    .ToList();
            }
        }

        [NotMapped]
        public ICollection<BudgetLine> UnallocatedIncomes {
            get {
                return UnallocatedLines
                    .Where(line => line.BudgetCategory.CategoryType == CategoryType.INCOME)
                    .OrderBy(line => line.BudgetCategory.ParentCategory?.Name ?? "")
                        .ThenBy(line => line.BudgetCategory.Name)
                    .ToList();
            }
        }

        [NotMapped]
        public ICollection<BudgetLine> UnallocatedExpenses {
            get {
                return UnallocatedLines
                    .Where(line => line.BudgetCategory.CategoryType == CategoryType.EXPENSE)
                    .OrderBy(line => line.BudgetCategory.ParentCategory?.Name ?? line.BudgetCategory.Name)
                        .ThenBy(line => line.BudgetCategory.Name)
                    .ToList();
            }
        }

        [NotMapped]
        public ICollection<BudgetLine> RolloverExpenses {
            get {
                return BudgetLines.Where(line => line.DoRollover).ToList();
            }
        }

        [NotMapped]
        public string TimespanString {
            get {
                return Timespan.GetAttribute<DisplayAttribute>().Name;
            }
        }
    }
}
