using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class TransactionSplitLines : AbstractTriggerHandler<Transaction> {
        public TransactionSplitLines(KoinzContext context, ILogger logger) : base(context, logger) { }

        public void DeleteSplitChildren(List<Transaction> oldList) {
            HashSet<Guid> parentTransactionIds = oldList.Select(trans => trans.Id).ToHashSet();

            if (parentTransactionIds.Count == 0) {
                return;
            }

            var childTransactions = context.Transactions
                .Where(trans => trans.ParentTransactionId != null)
                .Where(trans => parentTransactionIds.Contains(trans.ParentTransactionId.Value))
                .ToList();

            context.Transactions.RemoveRange(childTransactions);
        }

        public void UpdateSplitChildren(List<Transaction> oldList, List<Transaction> newList) {
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
            var splitTransactionDict = context.Transactions
                .Where(trans => parentDict.Keys.Contains(trans.ParentTransactionId.Value))
                .GroupBy(trans => trans.ParentTransactionId)
                .ToDictionary(x => x.Key.Value, x => x.ToList());

            List<Transaction> childrenToUpdate = new();
            foreach (var parentId in parentDict.Keys) {
                Transaction parent = parentDict[parentId];
                List<Transaction> splitLines = new();

                var splitSuccess = splitTransactionDict.TryGetValue2(parent.Id, out splitLines);
                if (splitSuccess == Status.ERROR) {
                    modelState.AddModelError("ParentTransactionId", "Unable to find child transactions");
                } else {
                    if (splitLines.All(child => child.MerchantId == parent.MerchantId)) {
                        splitLines.ForEach(child => {
                            child.MerchantId = parent.MerchantId;
                            child.MerchantName = parent.MerchantName;
                        });
                        childrenToUpdate.AddRange(splitLines);
                    }
                }
            }

            context.Transactions.UpdateRange(childrenToUpdate);
        }
    }
}
