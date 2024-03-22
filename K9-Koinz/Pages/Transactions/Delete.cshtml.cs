using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transactions {
    public class DeleteModel : AbstractDeleteModel<Transaction> {
        public DeleteModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Transaction> QueryRecordAsync(Guid id) {
            return await _data.Transactions.GetDetailsAsync(id);
        }

        protected override async Task BeforeDeleteActionsAsync() {
            var splitTransactions = (await _data.Transactions
                .GetSplitLines(Record.Id))
                .SplitTransactions;

            if (splitTransactions.Count > 0) {
                _data.Transactions.Remove(splitTransactions);
            }
        }

        protected override async Task AdditionalDatabaseActionsAsync() {
            if (Record.SavingsGoalId.HasValue) {
                var goal = await _data.SavingsGoals.GetByIdAsync(Record.SavingsGoalId.Value);
                goal.SavedAmount -= Record.Amount;
            }

            if (Record.TransferId.HasValue) {
                var otherTransaction = _data.Transactions
                    .GetMatchingFromTransferPair(Record.TransferId.Value, Record.Id);

                if (otherTransaction != null) {
                    _data.Transactions.Remove(otherTransaction);
                }

                var transfer = await _data.Transfers.GetByIdAsync(Record.TransferId);
                if (!transfer.RepeatConfigId.HasValue) {
                    _data.Transfers.Remove(transfer);
                }
            }
        }
    }
}
