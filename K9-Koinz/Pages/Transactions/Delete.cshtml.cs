using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transactions {
    public class DeleteModel : AbstractDeleteModel<Transaction> {
        public DeleteModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Transaction> QueryRecordAsync(Guid id) {
            return await _data.TransactionRepository.GetDetailsAsync(id);
        }

        protected override async Task BeforeDeleteActionsAsync() {
            var splitTransactions = (await _data.TransactionRepository
                .GetSplitLines(Record.Id))
                .SplitTransactions;

            if (splitTransactions.Count > 0) {
                _data.TransactionRepository.Remove(splitTransactions);
            }
        }

        protected override async Task AdditionalDatabaseActionsAsync() {
            if (Record.SavingsGoalId.HasValue) {
                var goal = await _data.SavingsGoalRepository.GetByIdAsync(Record.SavingsGoalId.Value);
                goal.SavedAmount -= Record.Amount;
            }

            if (Record.TransferId.HasValue) {
                var otherTransaction = _data.TransactionRepository
                    .GetMatchingFromTransferPair(Record.TransferId.Value, Record.Id);

                if (otherTransaction != null) {
                    _data.TransactionRepository.Remove(otherTransaction);
                }

                var transfer = await _data.TransferRepository.GetByIdAsync(Record.TransferId);
                if (!transfer.RepeatConfigId.HasValue) {
                    _data.TransferRepository.Remove(transfer);
                }
            }
        }
    }
}
