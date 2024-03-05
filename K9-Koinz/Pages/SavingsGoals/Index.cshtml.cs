using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.SavingsGoals {
    public class IndexModel : AbstractDbPage {
        public IndexModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) {
        }

        public Dictionary<string, List<SavingsGoal>> SavingsGoalsDict { get; set; }

        public async Task<IActionResult> OnGetAsync() {
            SavingsGoalsDict = await _context.SavingsGoals
                .Include(goal => goal.Transactions)
                .AsNoTracking()
                .GroupBy(goal => goal.AccountName)
                .ToDictionaryAsync(
                    x => x.Key,
                    x => x.AsEnumerable().OrderBy(goal => goal.Name).ToList()
                );

            VerifyGoalAmountsWithTransactions();

            return Page();
        }

        private void VerifyGoalAmountsWithTransactions() {
            var goalsToFix = new List<SavingsGoal>();
            foreach (var goal in SavingsGoalsDict.Values.SelectMany(x => x)) {
                var transactionsTotal = goal.Transactions
                    .Sum(trans => trans.Amount);
                if (transactionsTotal != goal.SavedAmount) {
                    goal.SavedAmount = transactionsTotal;
                    goalsToFix.Add(goal);
                }
            }

            if (goalsToFix.Count > 0) {
                _context.SavingsGoals.UpdateRange(goalsToFix);
                _context.SaveChanges();
            }
        }
    }
}
