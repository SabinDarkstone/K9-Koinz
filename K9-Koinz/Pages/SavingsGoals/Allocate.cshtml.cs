using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.SavingsGoals {
    public class AllocateModel : AbstractDbPage {

        [BindProperty]
        public Transaction Transaction { get; set; }

        public SelectList GoalOptions { get; set; } = default!;

        public AllocateModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public async Task<IActionResult> OnGetAsync(Guid relatedId) {
            Transaction = await _data.Transactions.GetByIdAsync(relatedId);

            if (Transaction.IsSavingsSpending) {
                GoalOptions = _data.SavingsGoals.GetForDropdown(null);
            } else {
                GoalOptions = _data.SavingsGoals.GetForDropdown(Transaction.AccountId);
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
                var savingsGoal = await _data.SavingsGoals.GetByIdAsync(Transaction.SavingsGoalId);
                Transaction.SavingsGoalName = savingsGoal.Name;
            }

            var savingsGoalId = Transaction.SavingsGoalId;
            var oldTransaction = await _data.Transactions.GetByIdAsync(Transaction.Id);
            Transaction = oldTransaction;
            Transaction.SavingsGoalId = savingsGoalId;

            _data.Transactions.Update(Transaction);

            try {
                await _data.SaveAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!await _data.Transactions.DoesExistAsync(Transaction.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage(PagePaths.TransactionIndex);
        }
    }
}
