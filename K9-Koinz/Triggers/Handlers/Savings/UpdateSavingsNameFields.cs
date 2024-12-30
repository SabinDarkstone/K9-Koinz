using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Triggers.Handlers.Savings {
    public class UpdateSavingsNameFields : IHandler<SavingsGoal> {
        private readonly KoinzContext _context;
        public UpdateSavingsNameFields(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<SavingsGoal> oldList, List<SavingsGoal> newList) {
            var accountIds = newList.Select(x => x.AccountId).ToHashSet();
            var accountDict = _context.Accounts
                .AsNoTracking()
                .Where(acct => accountIds.Contains(acct.Id))
                .ToDictionary(x => x.Id, x => x.Name);

            foreach (var goal in newList) {
                if (!accountDict.ContainsKey(goal.AccountId)) {
                    continue;
                }

                goal.AccountName = accountDict[goal.AccountId];
            }
        }
    }
}
