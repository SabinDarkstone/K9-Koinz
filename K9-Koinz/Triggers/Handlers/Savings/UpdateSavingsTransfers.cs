using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Triggers.Handlers.Savings {
    public class UpdateSavingsTransfers : IHandler<SavingsGoal> {
        private readonly KoinzContext _context;

        public UpdateSavingsTransfers(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<SavingsGoal> oldList, List<SavingsGoal> newList) {
            var goalIds = newList.Select(x => x.Id).ToHashSet();

            var recurringTransfers = _context.Transfers
                .AsNoTracking()
                .Where(fer => fer.SavingsGoalId.HasValue && goalIds.Contains(fer.SavingsGoalId.Value))
                .ToList();

            var goalDict = newList.ToDictionary(x => x.Id, x => x.AccountId);

            foreach (var transfer in recurringTransfers) {
                if (!goalDict.ContainsKey(transfer.SavingsGoalId.Value)) {
                    continue;
                }

                transfer.ToAccountId = goalDict[transfer.SavingsGoalId.Value];
            }
        }
    }
}
