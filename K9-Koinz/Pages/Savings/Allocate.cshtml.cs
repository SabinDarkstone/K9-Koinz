using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace K9_Koinz.Pages.Savings {
    public class AllocateModel : PageModel {

        private readonly KoinzContext _context;

        [BindProperty]
        public Transaction Transaction { get; set; }

        public SelectList GoalOptions { get; set; } = default!;

        public AllocateModel(KoinzContext context) {
            _context = context;
        }

        public IActionResult OnGet(Guid relatedId) {
            Transaction = _context.Transactions.Find(relatedId);

            if (Transaction.IsSavingsSpending) {
                GoalOptions = new SelectList(_context.SavingsGoals
                    .OrderBy(goal => goal.Name)
                    .ToList(), nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
            } else {
                GoalOptions = new SelectList(_context.SavingsGoals
                    .Where(goal => goal.AccountId == Transaction.AccountId)
                    .OrderBy(goals => goals.Name)
                    .ToList(), nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return NotFound();
            }

            if (Transaction.SavingsGoalId == Guid.Empty) {
                Transaction.SavingsGoalId = null;
            } else {
                var savingsGoal = await _context.SavingsGoals.FindAsync(Transaction.SavingsGoalId);
                Transaction.SavingsGoalName = savingsGoal.Name;
            }

            var savingsGoalId = Transaction.SavingsGoalId;
            var oldTransaction = await _context.Transactions.FindAsync(Transaction.Id);
            Transaction = oldTransaction;
            Transaction.SavingsGoalId = savingsGoalId;

            _context.Transactions.Update(Transaction);

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!TransactionExists(Transaction.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            var transactionFilterCookie = Request.Cookies["backToTransactions"].FromJson<TransactionNavPayload>();
            return RedirectToPage(PagePaths.TransactionIndex, routeValues: new {
                sortOrder = transactionFilterCookie.SortOrder,
                catFilter = transactionFilterCookie.CatFilter,
                pageIndex = transactionFilterCookie.PageIndex,
                accountFilter = transactionFilterCookie.AccountFilter,
                minDate = transactionFilterCookie.MinDate,
                maxDate = transactionFilterCookie.MaxDate,
                merchFilter = transactionFilterCookie.MerchFilter
            });
        }

        private bool TransactionExists(Guid id) {
            return _context.Transactions.Any(trans => trans.Id == id);
        }
    }
}
