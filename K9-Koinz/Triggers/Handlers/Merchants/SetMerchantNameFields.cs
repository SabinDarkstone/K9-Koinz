using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Merchants {
    public class SetMerchantNameFields : IHandler<Merchant> {
        private readonly KoinzContext _context;
        public SetMerchantNameFields(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Merchant> oldList, List<Merchant> newList) {
            var merchantIds = newList.Select(merchant => merchant.Id).ToList();

            var billsWithMerchant = _context.Bills
                .Where(bill => merchantIds.Contains(bill.MerchantId))
                .ToList();

            var transactionsWithMerchant = _context.Transactions
                .Where(trans => merchantIds.Contains(trans.MerchantId))
                .ToList();

            var merchantDict = newList.ToDictionary(merchant => merchant.Id, merchant => merchant);

            foreach (var bill in billsWithMerchant) {
                bill.MerchantName = merchantDict[bill.MerchantId].Name;
            }

            foreach (var trans in transactionsWithMerchant) {
                trans.MerchantName = merchantDict[trans.MerchantId].Name;
            }

            _context.Bills.UpdateRange(billsWithMerchant);
            _context.Transactions.UpdateRange(transactionsWithMerchant);
        }
    }
}
