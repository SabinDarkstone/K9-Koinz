using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class TransactionNameFields : AbstractTriggerHandler<Transaction> {
        public TransactionNameFields(KoinzContext context, ILogger logger) : base(context, logger) { }

        public void UpdateRelatedNameFields(List<Transaction> newTransactions) {
            if (modelState == null) {
                throw new Exception("ModelState cannot be null for UpdateRelatedNameFields");
            }

            HashSet<Guid> categoryIds = new();
            HashSet<Guid> merchantIds = new();
            HashSet<Guid> accountIds = new();
            HashSet<Guid> savingIds = new();

            foreach (var transaction in newTransactions) {
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
                categoryDict = context.Categories.Where(cat => categoryIds.Contains(cat.Id))
                    .ToDictionary(cat => cat.Id, cat => cat.Name);
            }

            if (merchantIds.Count > 0) {
                merchantDict = context.Merchants.Where(merch => merchantIds.Contains(merch.Id))
                    .ToDictionary(merch => merch.Id, merch => merch.Name);
            }

            if (accountIds.Count > 0) {
                accountDict = context.Accounts.Where(acct => accountIds.Contains(acct.Id))
                    .ToDictionary(acct => acct.Id, acct => acct.Name);
            }

            if (savingIds.Count > 0) {
                savingsDict = context.SavingsGoals.Where(sav => savingIds.Contains(sav.Id))
                    .ToDictionary(sav => sav.Id, sav => sav.Name);
            }

            foreach (var transaction in newTransactions) {
                var categoryName = "";
                var merchantName = "";
                var accountName = "";
                var savingsName = "";

                Status catSuccess = Status.NULL;
                Status merchSuccess = Status.NULL;
                Status acctSuccess = Status.NULL;
                Status savSuccess = Status.NULL;

                if (transaction.CategoryId != null) {
                    catSuccess = categoryDict.TryGetValue2(transaction.CategoryId.Value, out categoryName);
                }

                merchSuccess = merchantDict.TryGetValue2(transaction.MerchantId, out merchantName);
                acctSuccess = accountDict.TryGetValue2(transaction.AccountId, out accountName);

                if (transaction.SavingsGoalId != null) {
                    savSuccess = savingsDict.TryGetValue2(transaction.SavingsGoalId.Value, out savingsName);
                }

                if (catSuccess == Status.ERROR) {
                    modelState.AddModelError("CategoryId", "Invalid category selection");
                } else {
                    transaction.CategoryName = categoryName;
                }

                if (merchSuccess == Status.ERROR) {
                    modelState.AddModelError("MerchantId", "Invalid merchant selection");
                } else {
                    transaction.MerchantName = merchantName;
                }

                if (acctSuccess == Status.ERROR) {
                    modelState.AddModelError("AccountId", "Invalid account selection");
                } else {
                    transaction.AccountName = accountName;
                }

                if (savSuccess == Status.ERROR) {
                    modelState.AddModelError("SavingsGoalId", "Invalid savings selection");
                }
            }
        }
    }
}