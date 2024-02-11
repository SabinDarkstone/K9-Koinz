using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Categories {
    public class DeleteModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(KoinzContext context, ILogger<DeleteModel> logger) {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id, bool? saveChangesError = false) {
            if (id == null) {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(cat => cat.ChildCategories)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null) {
                return NotFound();
            }

            Category = category;

            if (saveChangesError.GetValueOrDefault()) {
                ErrorMessage = string.Format("Delete {id} failed. Try again.", id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(cat => cat.ChildCategories)
                .FirstAsync(merch => merch.Id == id);

            if (category == null) {
                return NotFound();
            }

            Category = category;

            try {
                _context.Categories.RemoveRange(Category.ChildCategories);
                _context.Categories.Remove(Category);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            } catch (DbUpdateException ex) {
                _logger.LogError(ex, ErrorMessage);
                return RedirectToAction("./Delete", new { id, saveChangesError = true });
            }
        }
    }
}
