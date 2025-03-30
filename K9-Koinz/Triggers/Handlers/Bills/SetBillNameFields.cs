using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Bills {
    public class SetBillNameFields : IHandler<Bill> {
        private readonly KoinzContext _context;
        public SetBillNameFields(KoinzContext context) {
            this._context = context;
        }

        public void Execute(List<Bill> oldList, List<Bill> newList) {
            HashSet<Guid> categoryIds = new();
            HashSet<Guid> merchantIds = new();
            HashSet<Guid> accountsIds = new();
            HashSet<Guid> savingsIds = new();

            foreach (var bill in newList) {
                if (bill.CategoryId != null) {
                    categoryIds.Add(bill.CategoryId.Value);
                }
                if (bill.SavingsGoalId != null) {
                    savingsIds.Add(bill.SavingsGoalId.Value);
                }
                merchantIds.Add(bill.MerchantId);
                accountsIds.Add(bill.AccountId);
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

            if (accountsIds.Count > 0) {
                accountDict = _context.Accounts.Where(acct => accountsIds.Contains(acct.Id))
                    .ToDictionary(acct => acct.Id, acct => acct.Name);
            }

            if (savingsIds.Count > 0) {
                savingsDict = _context.SavingsGoals.Where(goal => savingsIds.Contains(goal.Id))
                    .ToDictionary(goal => goal.Id, goal => goal.Name);
            }

            foreach (var bill in newList) {
                var categoryName = "";
                var merchantName = "";
                var accountName = "";
                var savingsName = "";

                if (bill.CategoryId != null) {
                    _ = categoryDict.TryGetValue2(bill.CategoryId.Value, out categoryName);
                }

                if (bill.SavingsGoalId != null) {
                    _ = savingsDict.TryGetValue2(bill.CategoryId.Value, out savingsName);
                }

                _ = merchantDict.TryGetValue2(bill.MerchantId, out merchantName);
                _ = accountDict.TryGetValue2(bill.AccountId, out accountName);

                bill.CategoryName = categoryName;
                bill.MerchantName = merchantName;
                bill.AccountName = accountName;
                bill.SavingsGoalName = savingsName;

                bill.RepeatConfig.DoRepeat = bill.IsRepeatBill;

                if (bill.RepeatConfig.Mode == RepeatMode.SPECIFIC_DAY) {
                    bill.RepeatConfig.IntervalGap = null;
                }
            }
        }
    }
}
