using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class UpdateSplitTransactions : IHandler<Transaction> {
        private readonly KoinzContext _context;
        public UpdateSplitTransactions(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Transaction> oldList, List<Transaction> newList) {
            Dictionary<Guid, Transaction> parentDict = new();

            foreach (var transaction in newList) {
                if (transaction.IsSplit) {
                    parentDict.Add(transaction.Id, transaction);
                }
            }

            if (parentDict.Count == 0) {
                return;
            }

            // This contains key/value pairs of parent transaction Ids to a list of split transactions
            var splitTransactionDict = _context.Transactions
                .Where(trans => parentDict.Keys.Contains(trans.ParentTransactionId.Value))
                .GroupBy(trans => trans.ParentTransactionId)
                .ToDictionary(x => x.Key.Value, x => x.ToList());

            List<Transaction> childrenToUpdate = new();
            foreach (var parentId in parentDict.Keys) {
                Transaction parent = parentDict[parentId];
                List<Transaction> splitLines = new();

                var _ = splitTransactionDict.TryGetValue2(parent.Id, out splitLines);
                if (splitLines.All(child => child.MerchantId == parent.MerchantId)) {
                    splitLines.ForEach(child => {
                        child.MerchantId = parent.MerchantId;
                        child.MerchantName = parent.MerchantName;
                    });
                    childrenToUpdate.AddRange(splitLines);
                }
            }

            _context.Transactions.UpdateRange(childrenToUpdate);
        }
    }
}
