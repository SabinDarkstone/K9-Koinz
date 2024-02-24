using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transactions {
    public class DeleteModel : AbstractDeleteModel<Transaction> {
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<Transaction> QueryRecordAsync(Guid id) {
            return await _context.Transactions
                .Include(trans => trans.Tag)
                .FirstOrDefaultAsync(trans => trans.Id == id);
        }

        protected override void AdditioanlDatabaseActions() {
            if (Record.SavingsGoalId.HasValue) {
                var goal = _context.SavingsGoals.Find(Record.SavingsGoalId);
                goal.SavedAmount -= Record.Amount;
            }
        }
    }
}
