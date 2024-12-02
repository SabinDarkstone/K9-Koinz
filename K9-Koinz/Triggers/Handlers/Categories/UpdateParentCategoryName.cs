using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Categories {
    public class UpdateParentCategoryName : IHandler<Category> {
        private readonly KoinzContext _context;

        public UpdateParentCategoryName(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Category> oldList, List<Category> newList) {
            var updatedCategoryIds = newList.Select(cat => cat.Id).ToHashSet();
            var childCategories = _context.Categories
                .Where(cat => updatedCategoryIds.Contains(cat.ParentCategoryId.Value))
                .ToList();
            foreach (var cat in childCategories) {
                cat.ParentCategoryName = newList.First(c => c.Id == cat.ParentCategoryId).Name;
            }

            _context.Categories.UpdateRange(childCategories);
        }
    }
}
