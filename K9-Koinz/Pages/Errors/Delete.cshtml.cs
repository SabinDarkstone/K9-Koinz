using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Errors {
    public class DeleteModel : PageModel {
        private readonly KoinzContext _context;

        public DeleteModel(KoinzContext context) {
            _context = context;
        }

        [BindProperty]
        public ErrorLog ErrorLog { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var errorlog = await _context.Errors.FirstOrDefaultAsync(m => m.Id == id);

            if (errorlog is not null) {
                ErrorLog = errorlog;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var errorlog = await _context.Errors.FindAsync(id);
            if (errorlog != null) {
                ErrorLog = errorlog;
                _context.Errors.Remove(ErrorLog);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
