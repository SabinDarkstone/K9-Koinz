using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using K9_Koinz.Pages.Meta;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace K9_Koinz.Pages.Transfers {
    public class CreateModel : AbstractCreateModel<Transfer> {
        private Transaction[] transactions = new Transaction[2];
        private bool foundMatchingTransactions;

        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger, IDropdownPopulatorService dropdownService)
            : base(context, logger, dropdownService) { }

        protected override Task BeforePageLoadActions() {
            var defaultCategory = _context.Categories
                .Where(cat => cat.Name == "Transfer")
                .FirstOrDefault();
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
            transactions = (await _context.CreateTransactionsFromTransfer(Record, false)).ToArray();

            foundMatchingTransactions = _context.Transactions
                .Where(trans => trans.AccountId == transactions[0].AccountId)
                .Where(trans => trans.Amount == transactions[0].Amount)
                .ToList()
                .Where(trans => Math.Abs((trans.Date - transactions[1].Date).TotalDays) <= 5)
                .Any();

            foreach (var transaction in transactions) {
                transaction.TransferId = Record.Id;
            }

            _context.Transactions.AddRange(transactions);
            await _context.SaveChangesAsync();
        }

        protected override IActionResult NavigateOnSuccess() {
            if (foundMatchingTransactions) {
                return RedirectToPage(PagePaths.TransactionDuplicateFound, new { id = transactions[1].Id });
            }

            var toAccount = _context.Accounts.Find(transactions[1].AccountId);
            var accountHasGoals = _context.SavingsGoals.Any(goal => goal.AccountId == toAccount.Id);

            if ((toAccount.Type == AccountType.CHECKING || toAccount.Type == AccountType.SAVINGS) && accountHasGoals) {
                return RedirectToPage(PagePaths.SavingsGoalsAllocate, new { relatedId = transactions[1].Id });
            }

            return RedirectToPage(PagePaths.TransactionIndex);
        }
    }
}
