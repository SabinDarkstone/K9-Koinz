using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class TransactionNameFields : IHandler<Transaction> {
        private readonly KoinzContext _context;

        public TransactionNameFields(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Transaction> oldList, List<Transaction> newList) {
            HashSet<Guid> categoryIds = new();
            HashSet<Guid> merchantIds = new();
            HashSet<Guid> accountIds = new();
            HashSet<Guid> savingIds = new();

            foreach (var transaction in newList) {
                if (transaction.CategoryId != null) {
                    categoryIds.Add(transaction.CategoryId.Value);
                }
                merchantIds.Add(transaction.MerchantId);
                accountIds.Add(transaction.AccountId);

                if (transaction.TagId == Guid.Empty) {
                    transaction.TagId = null;
                }

                if (transaction.BillId == Guid.Empty) {
                    transaction.BillId = null;
                }

                if (!transaction.SavingsGoalId.HasValue || transaction.SavingsGoalId.Value == Guid.Empty) {
                    transaction.SavingsGoalId = null;
                } else {
                    savingIds.Add(transaction.SavingsGoalId.Value);
                }
            }

            Dictionary<Guid, string> categoryDict = new();
            Dictionary<Guid, string> merchantDict = new();
            Dictionary<Guid, string> accountDict = new();
            Dictionary<Guid, string> savingsDict = new();

            if (categoryIds.Count > 0) {
                categoryDict = _context.Categories.Where(cat => categoryIds.Contains(cat.Id))
                    .ToDictionary(cat => cat.Id, cat => cat.Name);
            }

            if (merchantIds.Count > 0) {
                merchantDict = _context.Merchants.Where(merch => merchantIds.Contains(merch.Id))
                    .ToDictionary(merch => merch.Id, merch => merch.Name);
            }

            if (accountIds.Count > 0) {
                accountDict = _context.Accounts.Where(acct => accountIds.Contains(acct.Id))
                    .ToDictionary(acct => acct.Id, acct => acct.Name);
            }

            if (savingIds.Count > 0) {
                savingsDict = _context.SavingsGoals.Where(sav => savingIds.Contains(sav.Id))
                    .ToDictionary(sav => sav.Id, sav => sav.Name);
            }

            foreach (var transaction in newList) {
                var categoryName = "";
                var merchantName = "";
                var accountName = "";
                var savingsName = "";

                if (transaction.CategoryId != null) {
                    _ = categoryDict.TryGetValue2(transaction.CategoryId.Value, out categoryName);
                }

                _ = merchantDict.TryGetValue2(transaction.MerchantId, out merchantName);
                _ = accountDict.TryGetValue2(transaction.AccountId, out accountName);

                if (transaction.SavingsGoalId != null) {
                    _ = savingsDict.TryGetValue2(transaction.SavingsGoalId.Value, out savingsName);
                    transaction.SavingsGoalName = savingsName;
                }

                transaction.CategoryName = categoryName;
                transaction.MerchantName = merchantName;
                transaction.AccountName = accountName;
            }
        }
    }
}