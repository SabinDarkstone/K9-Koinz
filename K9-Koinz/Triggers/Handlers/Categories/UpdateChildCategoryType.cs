using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Categories {
    public class UpdateChildCategoryType : IHandler<Category> {
        private readonly KoinzContext _context;

        public UpdateChildCategoryType(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Category> oldList, List<Category> newList) {
            var categoryIds = newList.Select(cat => cat.Id).ToHashSet();

            var childCategories = _context.Categories
                .Where(cat => categoryIds.Contains(cat.ParentCategoryId.Value))
                .ToList();

            foreach (var cat in childCategories) {
                cat.CategoryType = newList.FirstOrDefault(c => c.Id == cat.ParentCategoryId.Value).CategoryType;
            }

            _context.Categories.UpdateRange(childCategories);
        }
    }
}
