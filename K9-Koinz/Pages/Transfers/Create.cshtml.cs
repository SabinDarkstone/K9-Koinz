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

        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger, IAccountService accountService, IAutocompleteService autocompleteService, ITagService tagService)
            : base(context, logger, accountService, autocompleteService, tagService) { }

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

        protected override async Task BeforeSaveActionsAsync() {
            Record.Date = Record.Date.AtMidnight() + DateTime.Now.TimeOfDay;

            transactions = (await Record.CreateTransactions(_context, false)).ToArray();

            foundMatchingTransactions = _context.Transactions
                .Where(trans =>
                    (trans.AccountId == transactions[0].AccountId && trans.Amount == transactions[1].Amount) ||
                    (trans.AccountId == transactions[1].AccountId && trans.Amount == transactions[0].Amount)
                )
                .ToList()
                .Where(trans => Math.Abs((trans.Date - transactions[1].Date).TotalDays) <= 5)
                .Any();

            _context.Transactions.AddRange(transactions);
        }

        protected override async Task AfterSaveActionsAsync() {
            foreach (var transaction in transactions) {
                transaction.TransferId = Record.Id;
            }

            _context.Transactions.UpdateRange(transactions);
            await _context.SaveChangesAsync();
        }

        protected override IActionResult NavigateOnSuccess() {
            if (foundMatchingTransactions) {
                return RedirectToPage("/Transactions/DuplicateFound", new { id = transactions[1].Id });
            }

            var toAccount = _context.Accounts.Find(transactions[1].AccountId);
            var accountHasGoals = _context.SavingsGoals.Any(goal => goal.AccountId == toAccount.Id);

            if ((toAccount.Type == AccountType.CHECKING || toAccount.Type == AccountType.SAVINGS) && accountHasGoals) {
                return RedirectToPage("/SavingsGoals/Allocate", new { relatedId = transactions[1].Id });
            }

            return RedirectToPage("/Transactions/Index");
        }

        public async Task<IActionResult> OnGetMerchantAutoComplete(string text) {
            return await _autocompleteService.AutocompleteMerchantsAsync(text.Trim());
        }

        public async Task<IActionResult> OnGetCategoryAutoComplete(string text) {
            return await _autocompleteService.AutocompleteCategoriesAsync(text.Trim());
        }
    }
}
