using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class TransactionNameFields {
        public static void UpdateRelatedNameFields(KoinzContext context, ModelStateDictionary modelState, List<Transaction> newTransactions) {
            if (modelState == null) {
                throw new Exception("ModelState cannot be null for UpdateRelatedNameFields");
            }

            HashSet<Guid> categoryIds = new();
            HashSet<Guid> merchantIds = new();
            HashSet<Guid> accountIds = new();
            HashSet<Guid> tagdIds = new();

            foreach (var transaction in newTransactions) {
                if (transaction.CategoryId != null) {
                    categoryIds.Add(transaction.CategoryId.Value);
                }
                merchantIds.Add(transaction.MerchantId);
                accountIds.Add(transaction.AccountId);
                if (transaction.TagId == Guid.Empty) {
                    transaction.TagId = null;
                } else {
                    tagdIds.Add(transaction.TagId.Value);
                }
            }

            Dictionary<Guid, string> categoryDict = new();
            Dictionary<Guid, string> merchantDict = new();
            Dictionary<Guid, string> accountDict = new();

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

            foreach (var transaction in newTransactions) {
                var categoryName = "";
                var merchantName = "";
                var accountName = "";

                var catSuccess = false;
                var merchSuccess = false;
                var acctSuccess = false;

                if (transaction.CategoryId != null) {
                    catSuccess = categoryDict.TryGetValue(transaction.CategoryId.Value, out categoryName);
                }

                merchSuccess = merchantDict.TryGetValue(transaction.MerchantId, out merchantName);
                acctSuccess = accountDict.TryGetValue(transaction.AccountId, out accountName);

                if (!catSuccess) {
                    modelState.AddModelError<Transaction>(x => x.CategoryId, "Invalid category selection");
                } else {
                    transaction.CategoryName = categoryName;
                }

                if (!merchSuccess) {
                    modelState.AddModelError<Transaction>(x => x.MerchantId, "Invalid merchant selection");
                } else {
                    transaction.MerchantName = merchantName;
                }

                if (!acctSuccess) {
                    modelState.AddModelError<Transaction>(x => x.AccountId, "Invalid account selection");
                } else {
                    transaction.AccountName = accountName;
                }
            }
        }
    }
}