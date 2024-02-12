using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transactions {
    public class CreateModel : AbstractCreateModel<Transaction> {
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
        }
    }
}
