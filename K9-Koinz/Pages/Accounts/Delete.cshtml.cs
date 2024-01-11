using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Accounts {
    public class DeleteModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(KoinzContext context, ILogger<DeleteModel> logger) {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Account Account { get; set; } = default!;
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id, bool? saveChangesError = false) {
            if (id == null) {
                return NotFound();
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == id);

            if (account == null) {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault()) {
                ErrorMessage = string.Format("Delete {id} failed. Try again.", id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);

            if (account == null) {
                return NotFound();
            }

            Account = account;
            try {
                _context.Accounts.Remove(Account);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            } catch (DbUpdateException ex) {
                _logger.LogError(ex, ErrorMessage);
                return RedirectToAction("./Delete", new { id, saveChangesError = true });
            }
        }
    }
}
