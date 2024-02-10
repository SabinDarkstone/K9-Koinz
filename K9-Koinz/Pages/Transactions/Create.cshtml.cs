using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Transactions {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<CreateModel> _logger;
        private readonly IAccountService _accountService;
        private readonly IAutocompleteService _autocompleteService;
        private readonly ITagService _tagService;

        public CreateModel(KoinzContext context, ILogger<CreateModel> logger, IAccountService accountService,
            IAutocompleteService autocompleteService, ITagService tagService) {
            _context = context;
            _logger = logger;
            _accountService = accountService;
            _autocompleteService = autocompleteService;
            _tagService = tagService;
        }

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        public void OnGet() {
            AccountOptions = _accountService.GetAccountList(true);
            TagOptions = _tagService.GetTagList();
        }

        [BindProperty]
        public Transaction Transaction { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            Transaction.Date = Transaction.Date.AtMidnight().Add(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));

            var category = await _context.Categories.FindAsync(Transaction.CategoryId);
            var merchant = await _context.Merchants.FindAsync(Transaction.MerchantId);
            var account = await _context.Accounts.FindAsync(Transaction.AccountId);
            Transaction.CategoryName = category.Name;
            Transaction.MerchantName = merchant.Name;
            Transaction.AccountName = account.Name;

            if (Transaction.TagId == Guid.Empty) {
                Transaction.TagId = null;
            }

            _context.Transactions.Add(Transaction);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public IActionResult OnGetMerchantAutoComplete(string text) {
            return _autocompleteService.AutocompleteMerchants(text.Trim());
        }

        public IActionResult OnGetCategoryAutoComplete(string text) {
            return _autocompleteService.AutocompleteCategories(text.Trim());
        }
    }
}
