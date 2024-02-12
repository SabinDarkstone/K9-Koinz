using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.Bills {
    public class CreateModel : AbstractCreateModel<Bill> {
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
            var account = await _context.Accounts.FindAsync(Record.AccountId);
            var merchant = await _context.Merchants.FindAsync(Record.MerchantId);
            var category = await _context.Categories.FindAsync(Record.CategoryId);

            Record.AccountName = account.Name;
            Record.MerchantName = merchant.Name;
            Record.CategoryName = category.Name;
        }
    }
}
