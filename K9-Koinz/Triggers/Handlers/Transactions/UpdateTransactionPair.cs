using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class UpdateTransactionPair : IHandler<Transaction> {
        private readonly KoinzContext _context;

        public UpdateTransactionPair(KoinzContext context) {
            _context = context;
        }
        public void Execute(List<Transaction> oldList, List<Transaction> newList) {
            // This stores key/value pairs of transfer Id to transactions
            Dictionary<Guid, Transaction> transactionDict = new();

            HashSet<Guid> transactionIds = newList.Select(trans => trans.Id).ToHashSet();

            foreach (var transaction in newList) {
                if (transaction.TransferId != null && !transaction.IsSplit) {
                    transactionDict.Add(transaction.TransferId.Value, transaction);
                }
            }

            if (transactionDict.Count == 0) {
                return;
            }

            var otherTransactionDict = _context.Transactions
                .Where(trans => trans.TransferId != null)
                .Where(trans => !transactionIds.Contains(trans.Id))
                .Where(trans => transactionDict.Keys.Contains(trans.TransferId.Value))
                .ToDictionary(trans => trans.TransferId, trans => trans);

            List<Transaction> transactionsToUpdate = new();
            foreach (var transferId in transactionDict.Keys) {
                Transaction transPair = new();
                Transaction original = transactionDict[transferId];

                var ferSuccess = otherTransactionDict.TryGetValue2(transferId, out transPair);
                if (ferSuccess == Status.SUCCESS) {
                    transPair.Amount = -1 * original.Amount;
                    transPair.Date = original.Date;
                    transPair.Notes = original.Notes;
                    transPair.TagId = original.TagId;
                    transactionsToUpdate.Add(transPair);
                }
            }

            _context.Transactions.UpdateRange(transactionsToUpdate);
        }
    }
}
