using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.Bills {
    public class CreateModel : AbstractCreateModel<Bill> {
        public CreateModel(KoinzContext context, IAccountService accountService, IAutocompleteService autocompleteService, ITagService tagService)
            : base(context, accountService, autocompleteService, tagService) { }

        public IActionResult OnGetMerchantAutoComplete(string text) {
            return _autocompleteService.AutocompleteMerchants(text.Trim());
        }

        public IActionResult OnGetCategoryAutoComplete(string text) {
            return _autocompleteService.AutocompleteCategories(text.Trim());
        }

        protected override async Task AfterSaveActions() {
            return;
        }

        protected override async Task BeforeSaveActions() {
            var account = await _context.Accounts.FindAsync(Record.AccountId);
            var merchant = await _context.Merchants.FindAsync(Record.MerchantId);
            var category = await _context.Categories.FindAsync(Record.CategoryId);

            Record.AccountName = account.Name;
            Record.MerchantName = merchant.Name;
            Record.CategoryName = category.Name;
        }
    }
}
