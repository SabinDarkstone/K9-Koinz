using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9_Koinz.Models {
    public enum CategoryType {
        [Display(Name = "Income")]
        INCOME,
        [Display(Name = "Expense")]
        EXPENSE,
        [Display(Name = "Transfer")]
        TRANSFER,
        [Display(Name = "Other")]
        OTHER,
        [Display(Name = "All")]
        ALL,
        UNASSIGNED
    }

    public class Category : BaseEntity, INameable {
        public string Name { get; set; }
        [DisplayName("Parent Category")]
        public Guid? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        public string ParentCategoryName { get; set; }
        [DisplayName("Category Type")]
        public CategoryType CategoryType { get; set; } = CategoryType.UNASSIGNED;

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<BudgetLine> BudgetLines { get; set; }
        public ICollection<Bill> Bills { get; set; }
        [DisplayName("Child Categories")]
        public ICollection<Category> ChildCategories { get; set; } = new List<Category>();

        [NotMapped]
        public bool IsChildCategory {
            get {
                return ParentCategoryId.HasValue;
            }
        }

        [NotMapped]
        public string FullyQualifiedName {
            get {
                var longName = string.Empty;
                if (ParentCategory != null) {
                    longName += ParentCategoryName + ": ";
                }
                longName += Name;

                return longName;
            }
        }

        [NotMapped]
        public string CategoryTypeName {
            get {
                return CategoryType.GetAttribute<DisplayAttribute>()?.Name ?? string.Empty;
            }
        }
    }
}
