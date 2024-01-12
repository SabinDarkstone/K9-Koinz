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

namespace K9_Koinz.Pages.Merchants {
    public class EditModel : PageModel {
        private readonly KoinzContext _context;

        public EditModel(KoinzContext context) {
            _context = context;
        }

        [BindProperty]
        public Merchant Merchant { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var merchant = await _context.Merchants.FirstOrDefaultAsync(m => m.Id == id);
            if (merchant == null) {
                return NotFound();
            }
            Merchant = merchant;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            _context.Attach(Merchant).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!MerchantExists(Merchant.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool MerchantExists(Guid id) {
            return _context.Merchants.Any(e => e.Id == id);
        }
    }
}
