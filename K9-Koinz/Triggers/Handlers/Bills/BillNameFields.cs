using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Bills {
    public class BillNameFields : AbstractTriggerHandler<Bill> {
        public BillNameFields(KoinzContext context, ILogger logger) : base(context, logger) { }

        public void UpdateRealtedNameFields(List<Bill> newBills) {
            if (modelState == null) {
                throw new Exception("ModelState cannot be null for UpdateRelatedNameFields");
            }

            HashSet<Guid> categoryIds = new();
            HashSet<Guid> merchantIds = new();
            HashSet<Guid> accountsIds = new();

            foreach (var bill in newBills) {
                if (bill.CategoryId != null) {
                    categoryIds.Add(bill.CategoryId.Value);
                }
                merchantIds.Add(bill.MerchantId);
                accountsIds.Add(bill.AccountId);
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

            if (accountsIds.Count > 0) {
                accountDict = context.Accounts.Where(acct => accountsIds.Contains(acct.Id))
                    .ToDictionary(acct => acct.Id, acct => acct.Name);
            }

            foreach (var bill in newBills) {
                var categoryName = "";
                var merchantName = "";
                var accountName = "";

                Status catSuccess = Status.NULL;
                Status merchSuccess = Status.NULL;
                Status acctSuccess = Status.NULL;

                if (bill.CategoryId != null) {
                    catSuccess = categoryDict.TryGetValue2(bill.CategoryId.Value, out categoryName);
                }

                merchSuccess = merchantDict.TryGetValue2(bill.MerchantId, out merchantName);
                acctSuccess = accountDict.TryGetValue2(bill.AccountId, out accountName);

                if (catSuccess == Status.ERROR) {
                    modelState.AddModelError("CategoryId", "Invalid category selection");
                } else {
                    bill.CategoryName = categoryName;
                }

                if (merchSuccess == Status.ERROR) {
                    modelState.AddModelError("MerchantId", "Invalid merchant selection");
                } else {
                    bill.MerchantName = merchantName;
                }

                if (acctSuccess == Status.ERROR) {
                    modelState.AddModelError("AccountId", "Invalid account selection");
                } else {
                    bill.AccountName = accountName;
                }

                bill.RepeatConfig.DoRepeat = bill.IsRepeatBill;

                if (bill.RepeatConfig.Mode == RepeatMode.SPECIFIC_DAY) {
                    bill.RepeatConfig.IntervalGap = null;
                }
            }
        }
    }
}
