using K9_Koinz.Models;

namespace K9_Koinz.Utils {
    public static class TransactionService {

        public static double GetTotalSpent(this ICollection<Transaction> transactions) {
            return transactions.Sum(trans => trans.Amount) * -1;
        }
    }
}
