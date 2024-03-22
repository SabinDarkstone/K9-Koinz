using K9_Koinz.Models;

namespace K9_Koinz.Utils {
    public static class TransferExtensions {
        public static Transfer GetInstanceOfRecurring(this Transfer recurringTransfer) {
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
                Id = recurringTransfer.Id
            };
        }
    }
}
