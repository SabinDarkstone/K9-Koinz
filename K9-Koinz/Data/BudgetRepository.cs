using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class BudgetRepository : GenericRepository<Budget> {
        public BudgetRepository(KoinzContext context) : base(context) { }

        public async Task<IEnumerable<Budget>> GetAllAsync() {
             return await _context.Budgets
                .Include(bud => bud.BudgetTag)
                .OrderBy(bud => bud.SortOrder)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Budget> GetBudgetDetails(string budgetId) {
            var budgetIQ = _context.Budgets
                .Include(bud => bud.BudgetLines)
                    .ThenInclude(line => line.BudgetCategory)
                        .ThenInclude(cat => cat.Transactions)
                .Include(bud => bud.BudgetLines)
                    .ThenInclude(line => line.BudgetCategory)
                        .ThenInclude(cat => cat.ChildCategories)
                            .ThenInclude(cCat => cCat.Transactions)
                .Include(bud => bud.BudgetLines)
                    .ThenInclude(line => line.Periods)
                .Include(bud => bud.BudgetTag)
                .OrderBy(bud => bud.Id)
                .AsSplitQuery()
                .AsNoTracking();

            if (!string.IsNullOrEmpty(budgetId)) {
                return budgetIQ.FirstOrDefault(bud => bud.Id == Guid.Parse(budgetId));
            } else {
                return budgetIQ.FirstOrDefault();
            }
        }
    }
}