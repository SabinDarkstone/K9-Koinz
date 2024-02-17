using K9_Koinz.Models;

namespace K9_Koinz.Utils {
    public static class TransactionExtensions {

        public static TinyTransaction MakeTiny(this Transaction transaction) {
            return new TinyTransaction(transaction);
        }
    }
}
