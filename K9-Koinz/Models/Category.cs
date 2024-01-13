using K9_Koinz.Utils;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9_Koinz.Models {
    public enum CategoryType {
        INCOME,
        EXPENSE
    }

    public class Category : DateTrackedEntity, INameable {
        [Unique<Category>]
        public string Name { get; set; }
        public Guid? ParentCategoryId {  get; set; }
        public Category ParentCategory { get; set; }
        
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<BudgetLine> BudgetLines { get; set; }
        public ICollection<Category> ChildCategories { get; set; } = new List<Category>();

        [NotMapped]
        public CategoryType Type {
            get {
                if (Name.Contains("Income", StringComparison.CurrentCultureIgnoreCase) || (ParentCategoryId.HasValue && ParentCategory.Type == CategoryType.INCOME)) {
                    return CategoryType.INCOME;
                } else {
                    return CategoryType.EXPENSE;
                }
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
