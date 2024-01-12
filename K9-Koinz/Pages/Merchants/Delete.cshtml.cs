using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace K9_Koinz.Pages.Merchants {
    public class DeleteModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(KoinzContext context, ILogger<DeleteModel> logger) {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Merchant Merchant { get; set; } = default!;
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id, bool? saveChangesError = false) {
            if (id == null) {
                return NotFound();
            }

            var merchant = await _context.Merchants
                .Include(merch => merch.Transactions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (merchant == null) {
                return NotFound();
            }

            Merchant = merchant;

            if (saveChangesError.GetValueOrDefault()) {
                ErrorMessage = string.Format("Delete {id} failed. Try again.", id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var merchant = await _context.Merchants
                .FirstAsync(merch => merch.Id == id);

            if (merchant == null) {
                return NotFound();
            }

            Merchant = merchant;
            try {
                _context.Merchants.Remove(Merchant);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            } catch (DbUpdateException ex) {
                _logger.LogError(ex, ErrorMessage);
                return RedirectToAction("./Delete", new { id, saveChangesError = true });
            }
        }
    }
}
