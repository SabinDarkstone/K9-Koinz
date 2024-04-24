using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transactions.Split {
    public class EditModel : AbstractDbPage {
        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public SelectList SavingsGoalList { get; set; }

        [BindProperty]
        public Transaction SplitTransaction { get; set; }

        public IActionResult OnGet(Guid id) {
            var transaction = _context.Transactions.Find(id);

            if (transaction != null) {
                SplitTransaction = transaction;
            }

            var savingsGoals = _context.SavingsGoals
                .AsNoTracking()
                .Where(goal => goal.AccountId == SplitTransaction.AccountId)
                .ToList();

            if (savingsGoals.Count > 0) {
                SavingsGoalList = new SelectList(savingsGoals,
                    nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var beforeTransction = _context.Transactions.Find(SplitTransaction.Id);

            Guid savingsGoalId = SplitTransaction.SavingsGoalId.Value;
            string notes = SplitTransaction.Notes;

            // Change only the savings goal and notes
            SplitTransaction = beforeTransction;
            SplitTransaction.SavingsGoalId = savingsGoalId == Guid.Empty ? null : savingsGoalId;
            SplitTransaction.Notes = notes;

            _context.Transactions.Update(SplitTransaction);
            await _context.SaveChangesAsync();

            return RedirectToPage(PagePaths.TransactionDetails, new { id = SplitTransaction.Id });
        }
    }
}
