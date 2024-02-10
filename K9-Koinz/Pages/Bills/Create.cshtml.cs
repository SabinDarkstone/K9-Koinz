using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Pages.Bills {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;
        private readonly IAccountService _accountService;
        private readonly IAutocompleteService _autocompleteService;

        public CreateModel(KoinzContext context, IAccountService accountService, IAutocompleteService autocompleteService) {
            _context = context;
            _accountService = accountService;
            _autocompleteService = autocompleteService;
        }

        public IActionResult OnGet() {
            AccountOptions = _accountService.GetAccountList(true);

            return Page();
        }

        [BindProperty]
        public Bill Bill { get; set; } = default!;
        public List<SelectListItem> AccountOptions;

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var account = _context.Accounts.Find(Bill.AccountId);
            var merchant = _context.Merchants.Find(Bill.MerchantId);

            Bill.AccountName = account.Name;
            Bill.MerchantName = merchant.Name;

            _context.Bills.Add(Bill);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public IActionResult OnGetMerchantAutoComplete(string text) {
            return _autocompleteService.AutocompleteMerchants(text.Trim());
        }
    }
}
