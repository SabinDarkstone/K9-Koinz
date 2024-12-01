using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class RemoveTransactionFromGoal : IHandler<Transaction> {
        private readonly KoinzContext _context;

        public RemoveTransactionFromGoal(KoinzContext context) {
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

            foreach (Transaction transaction in transactionsWithSavings) {
                SavingsGoal savingsGoal = new();

                _ = savingsDict.TryGetValue2(transaction.SavingsGoalId.Value, out savingsGoal);
                savingsGoal.SavedAmount -= transaction.Amount;
            }

            _context.SavingsGoals.UpdateRange(savingsDict.Values);
        }
    }
}
