using K9_Koinz.Data;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Pages.Meta {
    public class EditPageModel<TEntity> : PageModel where TEntity : BaseEntity {
        protected readonly IRepository<TEntity> _repository;
        protected readonly IDropdownPopulatorService _dropdownService;

        [BindProperty]
        public TEntity Record { get; set; } = default!;

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        public EditPageModel(IRepository<TEntity> repository, IDropdownPopulatorService dropdownService) {
            _repository = repository;
            _dropdownService = dropdownService;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            AccountOptions = await _dropdownService.GetAccountListAsync();
            TagOptions = await _dropdownService.GetTagListAsync();

            if (!id.HasValue) {
                return NotFound();
            }
            var record = await QueryRecord(id.Value);
            if (record == null) {
                return NotFound();
            }
            Record = record;

            await AfterQueryActions();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var saveResult = await _repository.UpdateAsync(Record);
            return HandleNavigate(saveResult);
        }

        protected virtual async Task<TEntity> QueryRecord(Guid id) {
            return await _repository.GetByIdAsync(id);
        }

        protected virtual Task AfterQueryActions() {
            return Task.CompletedTask;
        }

        protected virtual IActionResult HandleNavigate(DbSaveResult saveResult) {
            return RedirectToPage("./Index");
        }
    }
}
