using K9_Koinz.Data.Repositories.Meta;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Pages.Meta {
    public class CreatePageModel<TEntity> : PageModel where TEntity : BaseEntity {
        protected readonly IRepository<TEntity> _repository;
        protected readonly IDropdownPopulatorService _dropdownService;

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        [BindProperty]
        public TEntity Record { get; set; }

        protected Guid? RelatedId { get; private set; }

        public CreatePageModel(IRepository<TEntity> repository, IDropdownPopulatorService dropdownService = null) {
            _repository = repository;
            _dropdownService = dropdownService;
        }

        public async Task<IActionResult> OnGetAsync() {
            if (_dropdownService != null) {
                AccountOptions = await _dropdownService.GetAccountListAsync();
                TagOptions = await _dropdownService.GetTagListAsync();
            }

            await OnPageLoadActionsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var saveResult = await _repository.AddAsync(Record);
            return HandleNavigate(saveResult);
        }

        protected virtual Task OnPageLoadActionsAsync() {
            return Task.CompletedTask;
        }

        protected virtual IActionResult HandleNavigate(DbSaveResult saveResult) {
            return RedirectToPage("./Index");
        }
    }
}
