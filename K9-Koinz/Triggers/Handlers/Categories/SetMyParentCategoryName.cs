using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Categories {
    public class SetMyParentCategoryName : IHandler<Category> {
        private readonly KoinzContext _context;

        public SetMyParentCategoryName(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Category> oldList, List<Category> newList) {
            var parentCategoryIds = newList.Select(cat => cat.ParentCategoryId).ToHashSet();
            var parentCategories = _context.Categories
                .Where(cat => parentCategoryIds.Contains(cat.Id))
                .ToList();

            foreach (var cat in newList) {
                if (parentCategoryIds.Any(p => p == cat.ParentCategoryId.Value)) {
                    cat.ParentCategoryName = parentCategories.First(c => c.Id == cat.ParentCategoryId).Name;
                }
            }
        }
    }
}
