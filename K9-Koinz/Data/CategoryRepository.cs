using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class CategoryRepository : GenericRepository<Category> {
        public CategoryRepository(KoinzContext context) : base(context) { }

        public async Task<Category> GetCategoryDetails(Guid id) {
            return await _context.Categories
                .Include(cat => cat.ParentCategory)
                .Include(cat => cat.ChildCategories)
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
    }
}