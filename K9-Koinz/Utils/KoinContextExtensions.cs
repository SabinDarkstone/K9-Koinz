using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Utils {
    public static class KoinContextExtensions {
        public static Transfer GetInstanceOfRecurring(this KoinzContext context, Transfer recurringTransfer) {
            return new Transfer {
                Amount = recurringTransfer.Amount,
                CategoryId = recurringTransfer.CategoryId,
                Date = recurringTransfer.RepeatConfig.NextFiring.Value,
                Notes = recurringTransfer.Notes,
                MerchantId = recurringTransfer.MerchantId,
                SavingsGoalId = recurringTransfer.SavingsGoalId,
                TagId = recurringTransfer.TagId,
                ToAccountId = recurringTransfer.ToAccountId,
                FromAccountId = recurringTransfer.FromAccountId,
                RecurringTransferId = recurringTransfer.Id
            };
        }

        public static async Task<Transaction[]> CreateTransactionsFromTransfer(this KoinzContext context, Transfer transfer, bool trustSavingsGoals = true) {
            var category = await context.Categories.FindAsync(transfer.CategoryId);
            var merchant = await context.Merchants.FindAsync(transfer.MerchantId);
            var fromAccount = await context.Accounts.FindAsync(transfer.FromAccountId);
            var toAccount = await context.Accounts.FindAsync(transfer.ToAccountId);

            if (transfer.TagId == Guid.Empty) {
                transfer.TagId = null;
            }

            Transaction fromTransaction = null;

            if (transfer.FromAccountId.HasValue) {
                fromTransaction = new Transaction {
                    AccountId = transfer.FromAccountId.Value,
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
            }

            Transaction toTransaction = new Transaction {
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
            };

            if (transfer.FromAccountId.HasValue) {
                toTransaction.TransferId = transfer.Id;
            }

            if (trustSavingsGoals && transfer.SavingsGoalId.HasValue) {
                var savingsGoal = await context.SavingsGoals.FindAsync(transfer.SavingsGoalId);
                toTransaction.SavingsGoalId = transfer.SavingsGoalId;
                toTransaction.SavingsGoalName = savingsGoal.Name;
            }

            // TODO: Handle duplicates with a new table that can be reviewed later on

            return [fromTransaction, toTransaction];
        }
    }
}
