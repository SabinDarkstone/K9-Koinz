using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Utils {
    public static class IRepositoryWrapperExtensions {
        public static async Task<Transaction[]> CreateTransactionsFromTransfer(this IRepositoryWrapper data, Transfer transfer, bool trustSavingsGoals = true) {
            var category = await data.CategoryRepository.GetByIdAsync(transfer.CategoryId);
            var merchant = await data.MerchantRepository.GetByIdAsync(transfer.MerchantId);
            var fromAccount = await data.AccountRepository.GetByIdAsync(transfer.FromAccountId);
            var toAccount = await data.AccountRepository.GetByIdAsync(transfer.ToAccountId);

            if (transfer.TagId == Guid.Empty) {
                transfer.TagId = null;
            }

            var fromTransaction = new Transaction {
                AccountId = transfer.FromAccountId,
                AccountName = fromAccount.Name,
                CategoryId = transfer.CategoryId,
                CategoryName = category.Name,
                MerchantId = transfer.MerchantId,
                MerchantName = merchant.Name,
                Amount = -1 * transfer.Amount,
                Notes = transfer.Notes,
                TagId = transfer.TagId,
                Date = transfer.Date,
                TransferId = transfer.Id
            };
            var toTransaction = new Transaction {
                AccountId = transfer.ToAccountId,
                AccountName = toAccount.Name,
                CategoryId = transfer.CategoryId,
                CategoryName = category.Name,
                MerchantId = transfer.MerchantId,
                MerchantName = merchant.Name,
                Amount = transfer.Amount,
                Notes = transfer.Notes,
                TagId = transfer.TagId,
                Date = transfer.Date,
                TransferId = transfer.Id
            };

            if (trustSavingsGoals && transfer.SavingsGoalId.HasValue) {
                var savingsGoal = await data.SavingsGoalRepository.GetByIdAsync(transfer.SavingsGoalId.Value);
                toTransaction.SavingsGoalId = transfer.SavingsGoalId;
                toTransaction.SavingsGoalName = savingsGoal.Name;
            }

            // TODO: Handle duplicates with a new table that can be reviewed later on

            return [fromTransaction, toTransaction];
        }
    }
}
