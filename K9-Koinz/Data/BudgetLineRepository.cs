using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class BudgetLineRepository : GenericRepository<BudgetLine> {
        public BudgetLineRepository(KoinzContext context) : base(context) { }

        public override async Task<BudgetLine> GetByIdAsync(Guid? id) {
            return await _context.BudgetLines
                .Include(line => line.Budget)
                .Include(line => line.BudgetCategory)
                .AsNoTracking()
                .SingleOrDefaultAsync(line => line.Id == id);
        }

        public async Task<IEnumerable<BudgetLine>> GetByCategory(Guid categoryId) {
            return await _context.BudgetLines
                .Where(line => line.BudgetCategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BudgetLine>> GetByBudget(Guid budgetId) {
            return await _context.BudgetLines
                .Include(line => line.Periods)
                .Where(line => line.BudgetId == budgetId)
                .OrderBy(line => line.BudgetCategoryName)
                .ToListAsync();
        }
    }
}