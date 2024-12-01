using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class DeleteSplitTransactions : IHandler<Transaction> {
        private readonly KoinzContext _context;

        public DeleteSplitTransactions(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Transaction> oldList, List<Transaction> newList) {
            HashSet<Guid> parentTransactionIds = oldList.Select(trans => trans.Id).ToHashSet();

            if (parentTransactionIds.Count == 0) {
                return;
            }

            var childTransactions = _context.Transactions
                .Where(trans => trans.ParentTransactionId != null)
                .Where(trans => parentTransactionIds.Contains(trans.ParentTransactionId.Value))
                .ToList();

            _context.Transactions.RemoveRange(childTransactions);
        }
    }
}
