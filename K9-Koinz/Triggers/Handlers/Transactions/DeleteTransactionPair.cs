using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class DeleteTransactionPair : IHandler<Transaction> {
        private readonly KoinzContext _context;

        public DeleteTransactionPair(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Transaction> oldList, List<Transaction> newList) {
            HashSet<Guid> transferIds = new();
            HashSet<Guid> transactionIds = oldList.Select(trans => trans.Id).ToHashSet();

            foreach (var transaction in oldList) {
                if (transaction.TransferId != null) {
                    transferIds.Add(transaction.TransferId.Value);
                }
            }

            if (transferIds.Count == 0) {
                return;
            }

            var otherTransactions = _context.Transactions
                .Where(trans => !transactionIds.Contains(trans.Id))
                .Where(trans => trans.TransferId != null)
                .Where(trans => transferIds.Contains(trans.TransferId.Value))
                .ToList();

            _context.Transactions.RemoveRange(otherTransactions);
        }
    }
}
