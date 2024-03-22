using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.SavingsGoals {
    public class IndexModel : AbstractDbPage {
        public IndexModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) {
        }

        public Dictionary<string, List<SavingsGoal>> SavingsGoalsDict { get; set; }

        public async Task<IActionResult> OnGetAsync() {
            SavingsGoalsDict = await _data.SavingsGoals.GetAllGroupedByAccount();
            VerifyGoalAmountsWithTransactions();

            return Page();
        }

        private void VerifyGoalAmountsWithTransactions() {
            var goalsToFix = new List<SavingsGoal>();
            foreach (var goal in SavingsGoalsDict.Values.SelectMany(x => x)) {
                var transactionsTotal = goal.Transactions.GetTotal();
                if (transactionsTotal != goal.SavedAmount) {
                    goal.SavedAmount = transactionsTotal;
                    goalsToFix.Add(goal);
                }
            }

            if (goalsToFix.Count > 0) {
                _data.SavingsGoals.Update(goalsToFix);
                _data.Save();
            }
        }
    }
}
