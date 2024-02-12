using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transactions {
    public class EditModel : AbstractEditModel<Transaction> {
        public EditModel(KoinzContext context, IAccountService accountService, IAutocompleteService autocompleteService, ITagService tagService)
            : base(context, accountService, autocompleteService, tagService) { }

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            AccountOptions = await _accountService.GetAccountList(true);
            TagOptions = await _tagService.GetTagList();

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null) {
                return NotFound();
            }
            Record = transaction;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

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

            _context.Attach(Record).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!TransactionExists(Record.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TransactionExists(Guid id) {
            return _context.Transactions.Any(e => e.Id == id);
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
