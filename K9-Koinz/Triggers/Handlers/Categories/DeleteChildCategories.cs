using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Categories {
    public class DeleteChildCategories : IHandler<Category> {
        private readonly KoinzContext _context;

        public DeleteChildCategories(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Category> oldList, List<Category> newList) {
            var categoryIds = newList.Select(cat => cat.Id).ToList();
            var childCategories = _context.Categories
                .Where(cat => categoryIds.Contains(cat.ParentCategoryId.Value))
                .ToList();

            _context.Categories.RemoveRange(childCategories);
        }
    }
}
