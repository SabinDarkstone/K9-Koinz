using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Transfers {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<CreateModel> _logger;
        private readonly IAccountService _accountService;
        private readonly ITagService _tagService;
        private readonly IAutocompleteService _autocompleteService;

        public CreateModel(KoinzContext context, ILogger<CreateModel> logger,
            IAccountService accountService, ITagService tagService,
            IAutocompleteService autocompleteService) {
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

            var foundMatchingTransactions = _context.Transactions
                .Where(trans =>
                    (trans.AccountId == toTransaction.AccountId && trans.Amount == toTransaction.Amount) ||
                    (trans.AccountId == fromTransaction.AccountId && trans.Amount == fromTransaction.Amount)
                )
                .ToList()
                .Where(trans => Math.Abs((trans.Date - toTransaction.Date).TotalDays) <= 5)
                .Any();

            _context.Transactions.AddRange(new List<Transaction> { fromTransaction, toTransaction });
            await _context.SaveChangesAsync();

            if (foundMatchingTransactions) {
                return RedirectToPage("/Transactions/DuplicateFound", new { id = toTransaction.Id });
            }

            var accountHasGoals = _context.SavingsGoals.Any(goal => goal.AccountId == toAccount.Id);

            if ((toAccount.Type == AccountType.CHECKING || toAccount.Type == AccountType.SAVINGS) && accountHasGoals) {
                return RedirectToPage("/SavingsGoals/Allocate", new { relatedId = toTransaction.Id });
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
