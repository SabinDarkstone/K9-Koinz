using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace K9_Koinz.Pages.Savings {
    [Authorize]
    public class AllocateRecurringModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<AllocateRecurringModel> _logger;

        [BindProperty]
        public Transfer Transfer { get; set; }
        public SelectList GoalOptions { get; set; } = default!;

        public AllocateRecurringModel(KoinzContext context, ILogger<AllocateRecurringModel> logger) {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet(Guid relatedId) {
            Transfer = _context.Transfers
                .Include(fer => fer.FromAccount)
                .Include(fer => fer.ToAccount)
                .Include(fer => fer.RepeatConfig)
                .Include(fer => fer.Merchant)
                .Include(fer => fer.Category)
                .Include(fer => fer.SavingsGoal)
                .FirstOrDefault(fer => fer.Id == relatedId);

            GoalOptions = new SelectList(_context.SavingsGoals
                .Where(goal => goal.AccountId == Transfer.ToAccountId)
                .OrderBy(goals => goals.Name)
                .ToList(), nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                _logger.LogInformation(
                    JsonConvert.SerializeObject(ModelState, Formatting.Indented)
                );
                return NotFound();
            }

            if (Transfer.SavingsGoalId == Guid.Empty) {
                Transfer.SavingsGoalId = null;
            }

            var savingsGoalId = Transfer.SavingsGoalId;
            var oldTransfer = await _context.Transfers.FindAsync(Transfer.Id);
            Transfer = oldTransfer;
            Transfer.SavingsGoalId = savingsGoalId;

            _context.Transfers.Update(Transfer);

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!TransferExists(Transfer.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage(PagePaths.TransferManage);
        }

        private bool TransferExists(Guid id) {
            return _context.Transfers.Any(fer => fer.Id == id);
        }
    }
}
