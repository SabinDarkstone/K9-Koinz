using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transfers {
    public class DuplicateFoundModel : AbstractDbPage {
        public Transfer Transfer {  get; set; }
        public IEnumerable<Transfer> MatchingTransfers { get; set; }

        public DuplicateFoundModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (!id.HasValue) {
                return NotFound();
            }

            Transfer = await _data.TransferRepository.GetDetails(id.Value);
            MatchingTransfers = await _data.TransferRepository.FindDuplicates(Transfer);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id, string mode) {
            var transfer = await _data.TransferRepository.GetByIdAsync(id);

            if (mode == "cancel") {
                _data.TransferRepository.Remove(transfer);
                await _data.SaveAsync();
                return RedirectToPage(PagePaths.TransferManage);
            }

            var toAccount = await _data.AccountRepository.GetByIdAsync(transfer.ToAccountId);
            var accountHasGoals = _data.SavingsGoalRepository.ExistsByAccountId(transfer.ToAccountId);

            if ((toAccount.Type == AccountType.CHECKING || toAccount.Type == AccountType.SAVINGS) && accountHasGoals) {
                return RedirectToPage(PagePaths.SavingsGoalsAllocateRecurring, new { relatedId = transfer.Id });
            }

            return RedirectToPage(PagePaths.TransferManage);
        }
    }
}