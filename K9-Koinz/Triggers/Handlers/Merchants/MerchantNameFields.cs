using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Merchants {
    public class MerchantNameFields : AbstractTriggerHandler<Merchant> {
        public MerchantNameFields(KoinzContext context) : base(context) { }

        public void UpdateMerchantNames(List<Merchant> merchantList) {
            var merchantIds = merchantList.Select(merchant => merchant.Id).ToList();

            var billsWithMerchant = context.Bills
                .Where(bill => merchantIds.Contains(bill.MerchantId))
                .ToList();

            var transactionsWithMerchant = context.Transactions
                .Where(trans => merchantIds.Contains(trans.MerchantId))
                .ToList();

            var merchantDict = merchantList.ToDictionary(merchant => merchant.Id, merchant => merchant);

            foreach (var bill in billsWithMerchant) {
                bill.MerchantName = merchantDict[bill.MerchantId].Name;
            }

            foreach (var trans in transactionsWithMerchant) {
                trans.MerchantName = merchantDict[trans.MerchantId].Name;
            }

            context.Bills.UpdateRange(billsWithMerchant);
            context.Transactions.UpdateRange(transactionsWithMerchant);
        }
    }
}
