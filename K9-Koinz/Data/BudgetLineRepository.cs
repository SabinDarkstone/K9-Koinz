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
                .SingleOrDefaultAsync();
        }
    }
}