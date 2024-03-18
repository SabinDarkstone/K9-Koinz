namespace K9_Koinz.Models.Meta {
    public interface IAmount {
        public double Amount { get; set; }
    }

    public static class IAmountExtensions {
        public static double GetTotal(this IEnumerable<IAmount> transactions, bool doInvert = false) {
            var total = transactions.Sum(trans => trans.Amount);
            if (doInvert) {
                total *= -1;
            }
            return total;
        }
    }
}
