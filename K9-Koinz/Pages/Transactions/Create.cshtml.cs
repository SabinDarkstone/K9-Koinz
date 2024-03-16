using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.Transactions {
    public class CreateModel : AbstractCreateModel<Transaction> {
        private readonly IDupeCheckerService<Transaction> _dupeChecker;

        private bool doHandleSavingsGoal;
        private bool foundMatchingTransaction;

        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService, IDupeCheckerService<Transaction> dupeChecker)
                : base(context, logger, dropdownService) {
            _dupeChecker = dupeChecker;
        }

        protected override async Task BeforeSaveActionsAsync() {
            Record.Date = Record.Date.AtMidnight().Add(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));

            var category = await _context.Categories.FindAsync(Record.CategoryId);
            var merchant = await _context.Merchants.FindAsync(Record.MerchantId);
            var account = await _context.Accounts.FindAsync(Record.AccountId);
            Record.CategoryName = category.Name;
            Record.MerchantName = merchant.Name;
            Record.AccountName = account.Name;

            if (Record.TagId == Guid.Empty) {
                Record.TagId = null;
            }

            var matches = await _dupeChecker.FindPotentialDuplicates(Record);
            foundMatchingTransaction = matches.Count > 0;
        }

        protected override async Task AfterSaveActionsAsync() {
            doHandleSavingsGoal = await CheckIfAvailableForSavingsGoal() || Record.IsSavingsSpending;
        }

        protected override IActionResult NavigateOnSuccess() {
            if (foundMatchingTransaction) {
                return RedirectToPage(PagePaths.TransactionDuplicateFound, new { id = Record.Id });
            } else if (doHandleSavingsGoal) {
                return RedirectToPage(PagePaths.SavingsGoalsAllocate, new { relatedId = Record.Id });
            } else {
                return base.NavigateOnSuccess();
            }
        }

        private async Task<bool> CheckIfAvailableForSavingsGoal() {
            if (Record.Category.CategoryType != CategoryType.TRANSFER && Record.Category.CategoryType != CategoryType.INCOME) {
                return false;
            }

            var account = await _context.Accounts.FindAsync(Record.AccountId);
            if (account.Type != AccountType.SAVINGS && account.Type != AccountType.CHECKING) {
                return false;
            }

            var accountHasGoals = _context.SavingsGoals.Any(goal => goal.AccountId == Record.AccountId);
            return accountHasGoals;
        }
    }
}
