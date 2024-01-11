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
            ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(cat => cat.Name), nameof(Category.Id), nameof(Category.Name));
            ViewData["MerchantId"] = new SelectList(_context.Merchants.OrderBy(merch => merch.Name), nameof(Merchant.Id), nameof(Merchant.Name));
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
        public IActionResult OnGetAutoComplete(string text) {
            var merchants = _context.Merchants.Where(merch => merch.Name.StartsWith(text)).Select(merch => new {
                label = merch.Name,
                val = merch.Id
            }).ToList();
            return new JsonResult(merchants);
        }
    }
}
