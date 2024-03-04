using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Utils;

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

            Transfer.Date = Transfer.Date.AtMidnight() + DateTime.Now.TimeOfDay;

            var transactions = await Transfer.CreateTransactions(_context, false);

            var foundMatchingTransactions = _context.Transactions
                .Where(trans =>
                    (trans.AccountId == transactions[0].AccountId && trans.Amount == transactions[1].Amount) ||
                    (trans.AccountId == transactions[1].AccountId && trans.Amount == transactions[0].Amount)
                )
                .ToList()
                .Where(trans => Math.Abs((trans.Date - transactions[1].Date).TotalDays) <= 5)
                .Any();

            _context.Transactions.AddRange(transactions);
            await _context.SaveChangesAsync();

            if (foundMatchingTransactions) {
                return RedirectToPage("/Transactions/DuplicateFound", new { id = transactions[1].Id });
            }

            var toAccount = await _context.Accounts.FindAsync(transactions[1].AccountId);

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
