using Humanizer;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class TransactionDateField : IHandler<Transaction> {
        private readonly KoinzContext _context;

        public TransactionDateField(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Transaction> oldList, List<Transaction> newList) {
            foreach (var transaction in newList) {
                transaction.Date = transaction.Date.AtMidnight() + DateTime.Now.TimeOfDay;
            }
        }
    }
}
