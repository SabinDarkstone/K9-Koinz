using Humanizer;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class TransactionDateField {
        public static void UpdateDateField(ModelStateDictionary modelState, List<Transaction> newTransactions) {
            foreach (var transaction in newTransactions) {
                transaction.Date = transaction.Date.AtMidnight() + DateTime.Now.TimeOfDay;
            }
        }
    }
}
