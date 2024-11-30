using K9_Koinz.Models;
using K9_Koinz.Triggers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data.Repositories {
    public class CategoryRepository : TriggeredRepository<Category> {
        public CategoryRepository(KoinzContext context, ITrigger<Category> trigger)
            : base(context, trigger) { }

        public async Task<SelectList> GetCategoriesForList() {
            return new SelectList(await _dbSet.OrderBy(cat => cat.Name).ToListAsync(),
                nameof(Category.Id), nameof(Category.Name));
        }

        public List<Category> GetChildCategories(Guid? categoryId) {
            if (categoryId == null) {
                return new List<Category>();
            }

            return _dbSet.AsNoTracking()
                .Where(cat => cat.ParentCategoryId == categoryId)
                .ToList();
        }
    }
}
