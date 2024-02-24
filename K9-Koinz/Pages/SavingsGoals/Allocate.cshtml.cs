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

            GoalOptions = new SelectList(_context.SavingsGoals
                .Where(goal => goal.AccountId == Transaction.AccountId)
                .ToList(), nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
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
