using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Models {
    public class Budget : BaseEntity, INameable {

        [Required]
        [DisplayName("Budget Name")]
        public string Name { get; set; } = "New Budget";

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Tag")]
        public Guid? BudgetTagId { get; set; }
        public Tag BudgetTag { get; set; }

        [DisplayName("Ignore Categories")]
        public bool DoNotUseCategories { get; set; }

        [NotMapped]
        [DisplayName("Budgeted Amount")]
        public double? BudgetedAmount { get; set; }

        [NotMapped]
        [DisplayName("Rollover Unspent Money")]
        public bool DoNoCategoryRollover { get; set; }

        public ICollection<BudgetLine> BudgetLines { get; set; } = new List<BudgetLine>();

        [NotMapped]
        public ICollection<BudgetLine> UnallocatedLines { get; set; } = new List<BudgetLine>();

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
    }
}
