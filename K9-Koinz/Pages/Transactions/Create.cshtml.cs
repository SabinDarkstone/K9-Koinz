using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Utils;
using NuGet.Protocol;

namespace K9_Koinz.Pages.Transactions {
    public class CreateModel : AbstractCreateModel<Transaction> {
        private bool doHandleSavingsGoal;
        private bool foundMatchingTransaction;

        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IAccountService accountService, IAutocompleteService autocompleteService,
            ITagService tagService)
                : base(context, logger, accountService, autocompleteService, tagService) { }

        public async Task<IActionResult> OnGetMerchantAutoComplete(string text) {
            return await _autocompleteService.AutocompleteMerchantsAsync(text.Trim());
        }

        public async Task<IActionResult> OnGetCategoryAutoComplete(string text) {
            return await _autocompleteService.AutocompleteCategoriesAsync(text.Trim());
        }

        public async Task<IActionResult> OnGetSuggestedCategory(string merchantId) {
            var transactionsByCategory = (await _context.Transactions
                .AsNoTracking()
                .Where(trans => trans.MerchantId == Guid.Parse(merchantId))
                .ToListAsync())
                .GroupBy(x => x.CategoryId)
                .FirstOrDefault();

            // Get the most commonly used category
            var category = await _context.Categories.FindAsync(transactionsByCategory.ToList().FirstOrDefault().CategoryId);

            if (category != null) {
                return new JsonResult(category);
            } else {
                return null;
            }
        }

        public async Task<JsonResult> OnGetAddMerchant(string text) {
            var isExisting = await _context.Merchants.Where(merc => merc.Name == text).AnyAsync();
            if (isExisting) {
                return new JsonResult("DUPLICATE");
            } else {
                var newMerchant = new Merchant { Name = text };
                try {
                    _context.Merchants.Add(newMerchant);
                    await _context.SaveChangesAsync();
                    return new JsonResult(newMerchant.Id.ToString());
                } catch (Exception ex) {
                    return new JsonResult("ERROR");
                }
            }
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

            foundMatchingTransaction = (await _context.Transactions
                .Where(trans => trans.Amount == Record.Amount)
                .Where(trans => trans.MerchantId == Record.MerchantId)
                .ToListAsync())
                .Where(trans => Math.Abs((trans.Date - Record.Date).TotalDays) <= 5)
                .Any();
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
