using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.SavingsGoals {
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
                    .Where(goal => goal.StartDate <= DateTime.Now && goal.TargetDate >= DateTime.Now)
                    .OrderBy(goal => goal.AccountName)
                    .ThenBy(goal => goal.Name)
                    .ToList(), nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
            } else {
                GoalOptions = new SelectList(_context.SavingsGoals
                    .Where(goal => goal.StartDate <= DateTime.Now && goal.TargetDate >= DateTime.Now)
                    .Where(goal => goal.AccountId == Transaction.AccountId)
                    .OrderBy(goal => goal.AccountName)
                    .ThenBy(goals => goals.Name)
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

            _context.Attach(Transaction).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!TransactionExists(Transaction.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("/Transactions/Index");
        }

        private bool TransactionExists(Guid id) {
            return _context.Transactions.Any(trans => trans.Id == id);
        }
    }
}
