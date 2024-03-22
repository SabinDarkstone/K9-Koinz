using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transfers {
    public class CreateModel : AbstractCreateModel<Transfer> {
        private Transaction[] transactions = new Transaction[2];
        private bool foundMatchingTransactions;

        public CreateModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger, IDropdownPopulatorService dropdownService)
            : base(data, logger, dropdownService) { }

        protected override Task BeforePageLoadActions() {
            var defaultCategory = _data.Categories.GetByName("Transfer");
            Record = new Transfer {
                Category = defaultCategory,
                CategoryId = defaultCategory.Id,
            };

            return base.BeforePageLoadActions();
        }

        protected override void BeforeSaveActions() {
            Record.Date = Record.Date.AtMidnight() + DateTime.Now.TimeOfDay;

            if (Record.TagId == Guid.Empty) {
                Record.TagId = null;
            }
        }

        protected override async Task AfterSaveActionsAsync() {
            transactions = (await _data.CreateTransactionsFromTransfer(Record, false)).ToArray();
            foundMatchingTransactions = (await _data.Transactions.FindDuplicatesFromTransfer(transactions)).Any();

            foreach (var transaction in transactions) {
                transaction.TransferId = Record.Id;
            }

            _data.Transactions.Add(transactions);
            await _data.SaveAsync();
        }

        protected override IActionResult NavigateOnSuccess() {
            if (foundMatchingTransactions) {
                return RedirectToPage(PagePaths.TransactionDuplicateFound, new { id = transactions[1].Id });
            }

            var toAccount = _data.Accounts.GetByIdAsync(transactions[1].AccountId).Result;
            var accountHasGoals = _data.SavingsGoals.ExistsByAccountId(toAccount.Id);

            if ((toAccount.Type == AccountType.CHECKING || toAccount.Type == AccountType.SAVINGS) && accountHasGoals) {
                return RedirectToPage(PagePaths.SavingsGoalsAllocate, new { relatedId = transactions[1].Id });
            }

            return RedirectToPage(PagePaths.TransactionIndex);
        }
    }
}
