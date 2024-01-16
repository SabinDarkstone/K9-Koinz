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
        TRANSFER
    }

    public class Category : DateTrackedEntity, INameable {
        [Unique<Category>]
        public string Name { get; set; }
        [DisplayName("Parent Category")]
        public Guid? ParentCategoryId {  get; set; }
        public Category ParentCategory { get; set; }
        [DisplayName("Category Tyoe")]
        public CategoryType? CategoryType { get; set; }
        
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<BudgetLine> BudgetLines { get; set; }
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
                if (ParentCategoryId.HasValue) {
                    longName += ParentCategory.Name + ": ";
                }
                longName += Name;

                return longName;
            }
        }

		public override int GetHashCode() {
			return Id.GetHashCode();
		}
	}
}
