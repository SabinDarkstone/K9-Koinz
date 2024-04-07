using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Savings.Buckets {
    public class DetailsModel : AbstractDetailsModel<SavingsGoal> {
        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<SavingsGoal> QueryRecordAsync(Guid id) {
            return await _context.SavingsGoals
                .Include(goal => goal.Transactions)
                .Where(goal => goal.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }
    }
}
