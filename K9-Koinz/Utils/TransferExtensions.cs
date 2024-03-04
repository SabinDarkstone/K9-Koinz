using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Utils {
    public static class TransferExtensions {
        public static async Task<List<Transaction>> CreateTransactions(this Transfer transfer, KoinzContext context, bool trustSavingsGoals = true) {
            Transaction[] transactons = new Transaction[2];

            var category = await context.Categories.FindAsync(transfer.CategoryId);
            var merchant = await context.Merchants.FindAsync(transfer.MerchantId);
            var fromAccount = await context.Accounts.FindAsync(transfer.FromAccountId);
            var toAccount = await context.Accounts.FindAsync(transfer.ToAccountId);
            var savingsGoal = await context.SavingsGoals.FindAsync(transfer.SavingsGoalId);

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
                Date = transfer.Date
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
                Date = transfer.Date
            };

            if (trustSavingsGoals) {
                toTransaction.SavingsGoalId = transfer.SavingsGoalId;
                toTransaction.SavingsGoalName = savingsGoal.Name;
            }

            // TODO: Handle duplicates with a new table that can be reviewed later on

            return [fromTransaction, toTransaction];
        }
    }
}
