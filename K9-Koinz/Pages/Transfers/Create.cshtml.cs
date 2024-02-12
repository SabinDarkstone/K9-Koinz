using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;
using Humanizer;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Transfers {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<CreateModel> _logger;
        private readonly IAccountService _accountService;
        private readonly ITagService _tagService;
        private readonly IAutocompleteService _autocompleteService;

        public CreateModel(KoinzContext context, ILogger<CreateModel> logger, IAccountService accountService, ITagService tagService, IAutocompleteService autocompleteService) {
            _context = context;
            _logger = logger;
            _accountService = accountService;
            _tagService = tagService;
            _autocompleteService = autocompleteService;
        }

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        public async Task OnGetAsync() {
            AccountOptions = await _accountService.GetAccountListAsync(true);
            TagOptions = await _tagService.GetTagListAsync();
        }

        [BindProperty]
        public Transfer Transfer { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            Transfer.Date = Transfer.Date.AtMidnight().Add(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));

            var category = await _context.Categories.FindAsync(Transfer.CategoryId);
            var merchant = await _context.Merchants.FindAsync(Transfer.MerchantId);
            var fromAccount = await _context.Accounts.FindAsync(Transfer.FromAccountId);
            var toAccount = await _context.Accounts.FindAsync(Transfer.ToAccountId);

            if (Transfer.TagId == Guid.Empty) {
                Transfer.TagId = null;
            }

            var fromTransaction = new Transaction {
                AccountId = Transfer.FromAccountId,
                AccountName = fromAccount.Name,
                CategoryId = Transfer.CategoryId,
                CategoryName = category.Name,
                MerchantId = Transfer.MerchantId,
                MerchantName = merchant.Name,
                Amount = -1 * Transfer.Amount,
                Notes = Transfer.Notes,
                TagId = Transfer.TagId,
                Date = Transfer.Date
            };
            var toTransaction = new Transaction {
                AccountId = Transfer.ToAccountId,
                AccountName = toAccount.Name,
                CategoryId = Transfer.CategoryId,
                CategoryName = category.Name,
                MerchantId = Transfer.MerchantId,
                MerchantName = merchant.Name,
                Amount = Transfer.Amount,
                Notes = Transfer.Notes,
                TagId = Transfer.TagId,
                Date = Transfer.Date
            };

            _context.Transactions.Add(fromTransaction);
            _context.Transactions.Add(toTransaction);
            await _context.SaveChangesAsync();

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
