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
                .Include(trans => trans.Transfer)
                    .ThenInclude(fer => fer.Transactions)
                .FirstOrDefaultAsync(trans => trans.Id == id);
        }

        protected override void AdditioanlDatabaseActions() {
            if (Record.SavingsGoalId.HasValue) {
                var goal = _context.SavingsGoals.Find(Record.SavingsGoalId);
                goal.SavedAmount -= Record.Amount;
            }

            if (Record.TransferId.HasValue) {
                var otherTransaction = _context.Transactions
                    .Where(trans => trans.TransferId == Record.TransferId)
                    .Where(trans => trans.Id != Record.Id)
                    .SingleOrDefault();

                if (otherTransaction != null) {
                    _context.Transactions.Remove(otherTransaction);
                }

                var tranfer = _context.Transfers.Find(Record.TransferId);
                if (!tranfer.RepeatConfigId.HasValue) {
                    _context.Transfers.Remove(tranfer);
                }
            }
        }
    }
}
