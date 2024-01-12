using K9_Koinz.Utils;

namespace K9_Koinz.Models {
    public class Category : INameable {
        public Guid Id { get; set; }
        [Unique<Category>]
        public string Name { get; set; }
        public Guid? ParentCategoryId {  get; set; }
        public Category ParentCategory { get; set; }
        
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<BudgetLine> BudgetLines { get; set; }
        public ICollection<Category> ChildCategories { get; set; } = new List<Category>();

		public override int GetHashCode() {
			return Id.GetHashCode();
		}
	}
}
