using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Transactions {
    public class EditModel : PageModel {
        private readonly KoinzContext _context;

        public EditModel(KoinzContext context) {
            _context = context;
        }

        [BindProperty]
        public Transaction Transaction { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(trans => trans.Merchant)
                .Include(trans => trans.Account)
                .Include(trans => trans.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null) {
                return NotFound();
            }
            Transaction = transaction;
            ViewData["AccountId"] = new SelectList(_context.Accounts.OrderBy(acct => acct.Name), nameof(Account.Id), nameof(Account.Name));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            _context.Attach(Transaction).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!TransactionExists(Transaction.Id)) {
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
            var categories = _context.Categories
                .Include(cat => cat.ParentCategory)
                .AsNoTracking()
                .AsEnumerable()
                .Where(cat => cat.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase) || (cat.ParentCategoryId.HasValue && cat.ParentCategory.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase)))
                .Select(cat => new {
                    label = cat.ParentCategoryId != null ? cat.ParentCategory.Name + ": " + cat.Name : cat.Name,
                    val = cat.Id
                }).ToList();
            return new JsonResult(categories);
        }
    }
}
