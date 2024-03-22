using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.SavingsGoals {
    public class AllocateRecurringModel : AbstractDbPage {
        [BindProperty]
        public Transfer Transfer { get; set; }
        public SelectList GoalOptions { get; set; } = default!;

        public AllocateRecurringModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public async Task<IActionResult> OnGetAsync(Guid relatedId) {
            Transfer = await _data.TransferRepository.GetDetails(relatedId);
            GoalOptions = _data.SavingsGoalRepository.GetForDropdown(Transfer.ToAccountId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return NotFound();
            }

            if (Transfer.SavingsGoalId == Guid.Empty) {
                Transfer.SavingsGoalId = null;
            }

            var savingsGoalId = Transfer.SavingsGoalId;
            var oldTransfer = await _data.TransferRepository.GetByIdAsync(Transfer.Id);
            Transfer = oldTransfer;
            Transfer.SavingsGoalId = savingsGoalId;

            _data.TransferRepository.Update(Transfer);

            try {
                await _data.SaveAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!await _data.TransferRepository.DoesExistAsync(Transfer.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage(PagePaths.TransferManage);
        }
    }
}
