using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using K9_Koinz.Pages.Meta;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

            var startDate = Record.Date.AddDays(-3);
            var endDate = Record.Date.AddDays(3);

            foundMatchingTransactions = await _context.Transactions
                .Where(trans => (trans.AccountId == transactions[0].AccountId && trans.Amount == transactions[0].Amount) || (transactions[1] != null && trans.AccountId == transactions[1].AccountId && trans.Amount == transactions[1].Amount))
                .Where(trans => trans.Date.Date >= startDate.Date && trans.Date.Date <= endDate.Date.Date)
                .AnyAsync();

            _context.Transactions.AddRange(transactions.Where(x => x != null));
            await _context.SaveChangesAsync();
        }

        protected override IActionResult NavigateOnSuccess() {
            if (foundMatchingTransactions) {
                var startDate = Record.Date.AddDays(-3);
                var endDate = Record.Date.AddDays(3);

                _logger.LogInformation(
                    JsonConvert.SerializeObject(
                        _context.Transactions
                            .Where(trans => (trans.AccountId == transactions[0].AccountId && trans.Amount == transactions[0].Amount) || (transactions[1] != null && trans.AccountId == transactions[1].AccountId && trans.Amount == transactions[1].Amount))
                            .Where(trans => trans.Date.Date >= startDate.Date && trans.Date.Date <= endDate.Date.Date)
                            .ToList()
                        , new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects, Formatting = Formatting.Indented }
                    )
                );
                return RedirectToPage(PagePaths.TransactionDuplicateFound, new { id = transactions[1].Id });
            }

            var toAccount = _context.Accounts.Find(transactions[1].AccountId);
            var accountHasGoals = _context.SavingsGoals.Any(goal => goal.AccountId == toAccount.Id);

            if ((toAccount.Type == AccountType.CHECKING || toAccount.Type == AccountType.SAVINGS) && accountHasGoals) {
                return RedirectToPage(PagePaths.SavingsAllocate, new { relatedId = transactions[1].Id });
            }

            return RedirectToPage(PagePaths.TransactionIndex);
        }
    }
}
