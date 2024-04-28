using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Savings.Goals {
    public class DetailsModel : AbstractDetailsModel<SavingsGoal> {
        public List<Transfer> ScheduledTransfers { get; set; }

        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<SavingsGoal> QueryRecordAsync(Guid id) {
            var transfers = await _context.Transfers
                .AsNoTracking()
                .Where(fer => fer.SavingsGoalId == id)
                .Where(fer => fer.RepeatConfigId.HasValue)
                .Include(fer => fer.RepeatConfig)
                .ToListAsync();

            ScheduledTransfers = transfers;
            return await _context.SavingsGoals
                .Include(goal => goal.Transactions)
                .Where(goal => goal.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }
    }
}
