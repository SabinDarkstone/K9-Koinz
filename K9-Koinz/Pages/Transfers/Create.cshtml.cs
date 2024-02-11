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

        public CreateModel(KoinzContext context, ILogger<CreateModel> logger, IAccountService accountService, ITagService tagService) {
            _context = context;
            _logger = logger;
            _accountService = accountService;
            _tagService = tagService;
        }

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        public void OnGet() {
            AccountOptions = _accountService.GetAccountList(true);
            TagOptions = _tagService.GetTagList();
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

        public IActionResult OnGetMerchantAutoComplete(string text) {
            text = text.Trim();
            var merchants = _context.Merchants
                .AsNoTracking()
                .AsEnumerable()
                .Where(merch => merch.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(merch => new {
                    label = merch.Name,
                    val = merch.Id
                }).ToList();
            return new JsonResult(merchants);
        }

        public IActionResult OnGetCategoryAutoComplete(string text) {
            text = text.Trim();
            var categories = _context.Categories
                .Include(cat => cat.ParentCategory)
                .AsNoTracking()
                .AsEnumerable()
                .Where(cat => cat.FullyQualifiedName.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(cat => new {
                    label = cat.ParentCategoryId != null ? cat.ParentCategory.Name + ": " + cat.Name : cat.Name,
                    val = cat.Id
                }).ToList();
            return new JsonResult(categories);
        }
    }
}
