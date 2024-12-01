using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class UpdateGoalAmounts : IHandler<Transaction> {
        private readonly KoinzContext _context;

        public UpdateGoalAmounts(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Transaction> oldList, List<Transaction> newList) {
            HashSet<Guid> savingIds = new();
            List<Transaction> transactionsWithSavings = new();

            foreach (Transaction transaction in oldList) {
                if (transaction.SavingsGoalId != null && transaction.SavingsGoalId != Guid.Empty) {
                    savingIds.Add(transaction.SavingsGoalId.Value);
                }
            }

            var savingsDict = _context.SavingsGoals.Where(sav => savingIds.Contains(sav.Id))
                .ToDictionary(sav => sav.Id, sav => sav);

            foreach (var transaction in transactionsWithSavings) {
                var oldTransaction = oldList.FirstOrDefault(trans => trans.Id == transaction.Id);

                SavingsGoal savingsGoal = new();

                _ = savingsDict.TryGetValue2(transaction.SavingsGoalId.Value, out savingsGoal);

                savingsGoal.SavedAmount -= oldTransaction.Amount;
                savingsGoal.SavedAmount += transaction.Amount;
            }

            _context.SavingsGoals.UpdateRange(savingsDict.Values);
        }
    }
}
