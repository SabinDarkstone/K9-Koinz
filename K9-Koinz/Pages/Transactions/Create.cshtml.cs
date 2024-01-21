using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;
using Humanizer;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.Transactions {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(KoinzContext context, ILogger<CreateModel> logger) {
            _context = context;
            _logger = logger;
        }

        public List<SelectListItem> AccountOptions;

        public void OnGet() {
            AccountOptions = AccountUtils.GetAccountList(_context, true);
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

            _context.Transactions.Add(Transaction);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
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
