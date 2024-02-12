using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Meta {
    public abstract class AbstractDeleteModel<T> : AbstractDbPage where T : BaseEntity {
        protected AbstractDeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        [BindProperty]
        public T Record { get; set; }
        public string ErrorMessage { get; set; }

        public virtual async Task<IActionResult> OnGetAsync(Guid? id, bool? saveChangedError = false) {
            if (!id.HasValue) {
                return NotFound();
            }

            var record = await _context.Set<T>().FindAsync(id.Value);
            if (record == null) {
                return NotFound();
            }

            Record = record;

            if (saveChangedError.GetValueOrDefault()) {
                ErrorMessage = string.Format("Delete {id} failed. Try again.", id);
            }

            return Page();
        }

        public virtual async Task<IActionResult> OnPostAsync(Guid? id) {
            if (!id.HasValue) {
                return NotFound();
            }

            var record = await _context.Set<T>().FindAsync(id);
            if (record == null) {
                return NotFound();
            }

            Record = record;
            try {
                _context.Set<T>().Remove(Record);
                await _context.SaveChangesAsync();
                return NavigateOnSuccess();
            } catch (DbUpdateException ex) {
                _logger.LogError(ex, ErrorMessage);
                return RedirectToAction("./Delete", new { id, saveChangedError = true });
            }
        }

        public virtual IActionResult NavigateOnSuccess() {
            return RedirectToPage("./Index");
        }
    }
}
