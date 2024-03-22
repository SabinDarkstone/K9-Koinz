using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class CategoryRepository : GenericRepository<Category> {
        public CategoryRepository(KoinzContext context) : base(context) { }

        public async Task<Category> GetCategoryDetails(Guid id) {
            return await _context.Categories
                .Include(cat => cat.ParentCategory)
                .Include(cat => cat.ChildCategories
                    .OrderBy(cCat => cCat.Name))
                .AsNoTracking()
                .SingleOrDefaultAsync(cat => cat.Id == id);
        }

        public async Task<IEnumerable<Category>> GetAll() {
            return await _context.Categories
                .AsNoTracking()
                .Include(cat => cat.ChildCategories)
                    .ThenInclude(cCat => cCat.Transactions)
                .Include(cat => cat.Transactions)
                .OrderBy(cat => cat.CategoryType)
                    .ThenBy(cat => cat.Name)
                .ToListAsync();
        }

        public async Task<SelectList> GetDropdown() {
            var categories = await _context.Categories.OrderBy(cat => cat.Name).ToListAsync();
            return new SelectList(categories, nameof(Category.Id), nameof(Category.Name));
        }

        public async Task<IEnumerable<Category>> GetChildrenAsync(Guid parentCategoryId) {
            return await _context.Categories
                .Where(cat => cat.ParentCategoryId == parentCategoryId)
                .OrderBy(cat => cat.Name)
                .ToListAsync();
        }

        public Category GetByName(string name) {
            return _context.Categories
                .Where(cat => cat.Name == name)
                .SingleOrDefault();
        }
    }
}