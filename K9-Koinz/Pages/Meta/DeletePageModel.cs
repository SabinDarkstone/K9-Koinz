using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace K9_Koinz.Pages.Meta {
    public class DeletePageModel<TEntity> : PageModel where TEntity : BaseEntity {
        protected readonly IRepository<TEntity> _repository;

        [BindProperty]
        public TEntity Record { get; set; }
        public string ErrorMessage { get; set; }

        public DeletePageModel(IRepository<TEntity> repository) {
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id, bool? saveChangesError = false) {
            if (!id.HasValue) {
                return NotFound();
            }
            var record = await QueryRecord(id.Value);
            if (record == null) {
                return NotFound();
            }
            Record = record;

            AfterQueryActions();

            if (saveChangesError.GetValueOrDefault()) {
                ErrorMessage = string.Format("Delete {0} failed. Try again.", id.Value.ToString());
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id) {
            if (!id.HasValue) {
                return NotFound();
            }
            var record = await _repository.GetByIdAsync(id.Value);
            if (record == null) {
                return NotFound();
            }

            var saveResult = await _repository.DeleteAsync(id.Value);
            if (saveResult.IsSuccess) {
                return NavigateOnSuccess();
            } else {
                return RedirectToPage("./Delete", new { id, saveChangesError = true });
            }
        }

        protected virtual async Task<TEntity> QueryRecord(Guid id) {
            return await _repository.GetByIdAsync(id);
        }

        protected virtual void AfterQueryActions() { }
        
        protected virtual IActionResult NavigateOnSuccess() {
            return RedirectToPage("./Index");
        }
    }
}
