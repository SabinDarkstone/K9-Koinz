using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using K9_Koinz.Models.Helpers;
using NuGet.Protocol;
using K9_Koinz.Triggers;

namespace K9_Koinz.Pages.Transactions {
    public class CreateModel : AbstractCreateModel<Transaction> {
        private readonly IDupeCheckerService<Transaction> _dupeChecker;

        private bool doHandleSavingsGoal;
        private bool foundMatchingTransaction;

        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService, IDupeCheckerService<Transaction> dupeChecker)
                : base(context, logger, dropdownService) {
            _dupeChecker = dupeChecker;
            trigger = new TransactionTrigger(context);
        }

        protected override async Task BeforeSaveActionsAsync() {
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
                return RedirectToPage(PagePaths.SavingsAllocate, new { relatedId = Record.Id });
            } else {
                var transactionFilterString = Request.Cookies["backToTransactions"];
                if (string.IsNullOrEmpty(transactionFilterString)) {
                    return RedirectToPage(PagePaths.TransactionIndex);
                }

                var transactionFilterCookie = transactionFilterString.FromJson<TransactionNavPayload>();
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
