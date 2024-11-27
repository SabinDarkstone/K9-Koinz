using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using K9_Koinz.Utils;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Pages.Savings {
    public class IndexModel : AbstractDbPage {
        public IndexModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) {
        }

        public Dictionary<string, List<SavingsGoal>> SavingsDict { get; set; }

        public SavingsType ActiveTab { get; set; }

        [DisplayName("Show All")]
        public bool ShowAll { get; set; } = false;

        public string ShowAllString {
            get {
                return ShowAll ? "yes" : "no";
            }
        }

        public async Task<IActionResult> OnGetAsync(string view, string viewAll) {
            if (string.IsNullOrEmpty(view) || view == "goals") {
                ActiveTab = SavingsType.GOAL;
            } else {
                ActiveTab = SavingsType.BUCKET;
            }

            if (viewAll == "yes") {
                ShowAll = true;
            } else {
                ShowAll = false;
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
                .AsSplitQuery()
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
