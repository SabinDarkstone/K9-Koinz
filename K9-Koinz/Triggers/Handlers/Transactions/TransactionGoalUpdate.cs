using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class TransactionGoalUpdate {
        public static void UpdateGoalForDelete(KoinzContext context, ModelStateDictionary modelState, List<Transaction> oldList) {
            HashSet<Guid> savingIds = new();
            List<Transaction> transactionsWithSavings = new();

            foreach (Transaction transaction in oldList) {
                if (transaction.SavingsGoalId != null && transaction.SavingsGoalId != Guid.Empty) {
                    savingIds.Add(transaction.SavingsGoalId.Value);
                }
            }

            var savingsDict = context.SavingsGoals.Where(sav => savingIds.Contains(sav.Id))
                .ToDictionary(sav => sav.Id, sav => sav);

            foreach (Transaction transaction in transactionsWithSavings) {
                var savSuccess = Status.NULL;
                SavingsGoal savingsGoal = new();

                savSuccess = savingsDict.TryGetValue2(transaction.SavingsGoalId.Value, out savingsGoal);

                if (savSuccess == Status.ERROR) {
                    modelState.AddModelError("SavingsGoalId", "Invalid savings goal selection");
                } else {
                    savingsGoal.SavedAmount -= transaction.Amount;
                }
            }

            context.SavingsGoals.UpdateRange(savingsDict.Values);
        }

        public static void UpdateGoalForUpdate(KoinzContext context, ModelStateDictionary modelState, List<Transaction> oldList, List<Transaction> newList) {
            HashSet<Guid> savingIds = new();
            List<Transaction> transactionsWithSavings = new();

            foreach (Transaction transaction in oldList) {
                if (transaction.SavingsGoalId != null && transaction.SavingsGoalId != Guid.Empty) {
                    savingIds.Add(transaction.SavingsGoalId.Value);
                }
            }

            var savingsDict = context.SavingsGoals.Where(sav => savingIds.Contains(sav.Id))
                .ToDictionary(sav => sav.Id, sav => sav);

            foreach (var transaction in transactionsWithSavings) {
                var oldTransaction = oldList.FirstOrDefault(trans => trans.Id == transaction.Id);

                var savSuccess = Status.NULL;
                SavingsGoal savingsGoal = new();

                savSuccess = savingsDict.TryGetValue2(transaction.SavingsGoalId.Value, out savingsGoal);

                if (savSuccess == Status.ERROR) {
                    modelState.AddModelError("SavingsGoalId", "Invalid savings goal selection");
                } else {
                    savingsGoal.SavedAmount -= oldTransaction.Amount;
                    savingsGoal.SavedAmount += transaction.Amount;
                }
            }

            context.SavingsGoals.UpdateRange(savingsDict.Values);
        }
    }
}
