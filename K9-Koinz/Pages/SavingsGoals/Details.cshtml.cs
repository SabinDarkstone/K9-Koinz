using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.SavingsGoals {
    public class DetailsModel : AbstractDetailsModel<SavingsGoal> {
        public DetailsModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<SavingsGoal> QueryRecordAsync(Guid id) {
            return await _context.SavingsGoals
                .Include(goal => goal.Transactions)
                .Where(goal => goal.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }
    }
}
