using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Categories {
    public class DeleteModel : AbstractDeleteModel<Category> {
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public override async Task<IActionResult> OnGetAsync(Guid? id, bool? saveChangesError = false) {
            if (id == null) {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(cat => cat.ChildCategories)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null) {
                return NotFound();
            }

            Record = category;

            if (saveChangesError.GetValueOrDefault()) {
                ErrorMessage = string.Format("Delete {id} failed. Try again.", id);
            }

            return Page();
        }

        public override async Task<IActionResult> OnPostAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(cat => cat.ChildCategories)
                .FirstAsync(merch => merch.Id == id);

            if (category == null) {
                return NotFound();
            }

            Record = category;

            try {
                _context.Categories.RemoveRange(Record.ChildCategories);
                _context.Categories.Remove(Record);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            } catch (DbUpdateException ex) {
                _logger.LogError(ex, ErrorMessage);
                return RedirectToAction("./Delete", new { id, saveChangesError = true });
            }
        }
    }
}
