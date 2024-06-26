using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transfers.Recurring {
    public class CreateModel : AbstractCreateModel<Transfer> {
        private Transaction[] transactions = new Transaction[2];
        private bool foundMatchingSchedule;

        public Category DefaultCategory { get; set; }

        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger, IDropdownPopulatorService dropdownService)
            : base(context, logger, dropdownService) { }

        protected override Task BeforePageLoadActions() {
            DefaultCategory = _context.Categories
                .Where(cat => cat.Name == "Transfer")
                .FirstOrDefault();
            Record = new Transfer();
            Record.CategoryId = DefaultCategory.Id;

            return base.BeforePageLoadActions();
        }

        protected override async Task BeforeSaveActionsAsync() {
            var category = await _context.Categories.FindAsync(Record.CategoryId);

            if (!Record.FromAccountId.HasValue && category.CategoryType != CategoryType.INCOME) {
                ModelState.AddModelError("income", "You must select a From Account unless you are specifying a source of income.");
                return;
            }

            foundMatchingSchedule = (await _context.Transfers
                .Where(fer => fer.ToAccountId == Record.ToAccountId && fer.FromAccountId == Record.FromAccountId)
                .Where(fer => fer.Amount == Record.Amount)
                .Where(fer => fer.RepeatConfigId.HasValue)
                .Where(fer => fer.RepeatConfig.FirstFiring == Record.RepeatConfig.FirstFiring)
                .Where(fer => fer.RepeatConfig.Mode == Record.RepeatConfig.Mode)
                .Where(fer => fer.RepeatConfig.IntervalGap == Record.RepeatConfig.IntervalGap)
                .Where(fer => fer.RepeatConfig.Frequency == Record.RepeatConfig.Frequency)
                .ToListAsync())
                .Where(fer => Math.Abs((fer.Date - Record.Date).TotalDays) <= 5)
                .Any();

            if (Record.TagId == Guid.Empty) {
                Record.TagId = null;
            }

            Record.RepeatConfig.DoRepeat = true;
        }

        protected override async Task AfterSaveActionsAsync() {
            if (Record.RepeatConfig.NeedsToFireImmediately) {
                transactions = (await _context.CreateTransactionsFromTransfer(Record, false)).ToArray();
                Record.RepeatConfig.FireNow();

                foreach (var transaction in transactions) {
                    transaction.TransferId = Record.Id;
                }

                _context.Transfers.Update(Record);
                _context.Transactions.AddRange(transactions.Where(x => x != null));

                await _context.SaveChangesAsync();
            }
        }

        protected override IActionResult NavigateOnSuccess() {
            if (foundMatchingSchedule) {
                return RedirectToPage(PagePaths.TransferDuplicateFound, new { id = Record.Id });
            }

            var toAccount = _context.Accounts.Find(Record.ToAccountId);
            var accountHasGoals = _context.SavingsGoals.Any(goal => goal.AccountId == Record.ToAccountId);

            if ((toAccount.Type == AccountType.CHECKING || toAccount.Type == AccountType.SAVINGS) && accountHasGoals) {
                return RedirectToPage(PagePaths.SavingsAllocateRecurring, new { relatedId = Record.Id });
            }

            return RedirectToPage(PagePaths.TransferManage);
        }
    }
}
