using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace K9_Koinz.Pages.Savings {
    public class IndexModel : AbstractDbPage {
        public IndexModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) {
        }

        public Dictionary<string, List<SavingsGoal>> SavingsDict { get; set; }

        public SavingsType ActiveTab { get; set; }

        public bool ShowAll { get; set; } = false;

        public async Task<IActionResult> OnPostAsync(bool? showAll, string view) {
            ShowAll = showAll ?? false;
            return await OnGetAsync(view);
        }

        public async Task<IActionResult> OnGetAsync(string view) {
            if (string.IsNullOrEmpty(view) || view == "goals") {
                ActiveTab = SavingsType.GOAL;
            } else {
                ActiveTab = SavingsType.BUCKET;
            }

            var savingsIQ = _context.SavingsGoals
                .AsNoTracking()
                .Include(goal => goal.Transactions
                    .OrderByDescending(trans => trans.Date))
                .Where(goal => goal.SavingsType == ActiveTab);

            if (!ShowAll) {
                savingsIQ = savingsIQ.Where(goal => goal.IsActive);
            }

            SavingsDict = await savingsIQ
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
            foreach (var goal in SavingsDict.Values.SelectMany(x => x)) {
                var transactionsTotal = goal.Transactions
                    .GetTotal();
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
