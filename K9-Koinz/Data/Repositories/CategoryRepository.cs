using K9_Koinz.Models;
using K9_Koinz.Triggers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data.Repositories {
    public class CategoryRepository : TriggeredRepository<Category> {
        public CategoryRepository(KoinzContext context, ITrigger<Category> trigger)
            : base(context, trigger) { }

        public async Task<double> GetAverageSpending(DateTime startDate, DateTime endDate, Guid categoryId) {
            var transactions = _context.Transactions.AsNoTracking()
                .Include(trans => trans.Category)
                .Where(trans => trans.CategoryId.HasValue && trans.BillId == null && trans.TransferId == null)
                .Where(trans => !trans.IsSplit && !trans.IsSavingsSpending)
                .Where(trans => trans.CategoryId == categoryId || trans.Category.ParentCategoryId == categoryId)
                .Where(trans => trans.Date.Date >= startDate.Date && trans.Date.Date <= endDate.Date);

            if (!transactions.Any()) {
                return 0;
            }
            

            return (await transactions.GroupBy(trans => trans.Date.Month)
                .ToDictionaryAsync(x => x.Key, x => x.Sum(trans => trans.Amount) * -1))
                .Values.Where(x => x > 0).Average();
        }

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

        public async Task<Category> GetParentCategory(Guid? categoryId) {
            if (categoryId.HasValue) {
                return await GetByIdAsync(categoryId.Value, false);
            } else {
                return null;
            }
        }

        public async Task<Category> GetCategoryWithFamily(Guid categoryId) {
            return await _dbSet.AsNoTracking()
                .Include(cat => cat.ParentCategory)
                .Include(cat => cat.ChildCategories.OrderBy(cCat => cCat.Name))
                .FirstOrDefaultAsync(cat => cat.Id == categoryId);
        }
    }
}
