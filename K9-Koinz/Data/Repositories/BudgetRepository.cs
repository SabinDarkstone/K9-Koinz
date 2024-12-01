using K9_Koinz.Models;
using K9_Koinz.Triggers;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data.Repositories {
    public class BudgetRepository : TriggeredRepository<Budget> {
        public BudgetRepository(KoinzContext context, ITrigger<Budget> trigger)
            : base(context, trigger) { }

        public List<BudgetLine> GetBudgetLinesByBudgetId(Guid budgetId) {
            return _context.BudgetLines
                .Where(line => line.BudgetId == budgetId)
                .OrderBy(line => line.BudgetCategoryName)
                .ToList();
        }

        public async Task<Budget> GetBudgetDetails(Guid budgetId) {
            return await _dbSet.AsSplitQuery()
                .Include(bud => bud.BudgetLines.OrderBy(line => line.BudgetCategoryName))
                    .ThenInclude(line => line.BudgetCategory)
                .FirstOrDefaultAsync(bud => bud.Id == budgetId);
        }
    }
}