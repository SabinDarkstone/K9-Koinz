using Humanizer;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class TransactionDateField : AbstractTriggerHandler<Transaction> {
        public TransactionDateField(KoinzContext context) : base(context) { }

        public void UpdateDateField(List<Transaction> newTransactions) {
            foreach (var transaction in newTransactions) {
                transaction.Date = transaction.Date.AtMidnight() + DateTime.Now.TimeOfDay;
            }
        }
    }
}
