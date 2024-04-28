using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Savings.Buckets {
    public class EditModel : AbstractEditModel<SavingsGoal> {
        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger, dropdownService) { }

        protected override async Task BeforeSaveActionsAsync() {
            var account = await _context.Accounts.FindAsync(Record.AccountId);

            Record.AccountName = account.Name;
        }

        protected override async Task AfterSaveActionsAsync() {
            var recurringTransfers = await _context.Transfers
                .Where(fer => fer.SavingsGoalId == Record.Id)
                .ToListAsync();

            foreach (var recurringTransfer in recurringTransfers) {
                recurringTransfer.ToAccountId = Record.AccountId;
                _context.Transfers.Update(recurringTransfer);
            }

            await _context.SaveChangesAsync();
        }

        protected override IActionResult NavigationOnSuccess() {
            return RedirectToPage(PagePaths.SavingsIndex, new { view = "buckets" });
        }
    }
}
