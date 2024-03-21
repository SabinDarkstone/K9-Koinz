using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Pages.Meta {
    public abstract class AbstractCreateModel<TEntity> : AbstractDbPage where TEntity : BaseEntity {
        protected readonly IDropdownPopulatorService _dropdownService;

        [BindProperty]
        public TEntity Record { get; set; } = default!;

        protected Guid? RelatedId { get; private set; }

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        protected AbstractCreateModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService) : base(data, logger) {
            _dropdownService = dropdownService;
        }

        public virtual async Task<IActionResult> OnGetAsync(Guid? relatedId) {
            RelatedId = relatedId;

            await BeforePageLoadActions();
            return Page();
        }

        protected virtual async Task BeforePageLoadActions() {
            AccountOptions = await _dropdownService.GetAccountListAsync();
            TagOptions = await _dropdownService.GetTagListAsync();
        }

        public virtual async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                await BeforePageLoadActions();
                return NavigationOnFailure();
            }

            await BeforeSaveActionsAsync();
            BeforeSaveActions();

            _data.GetGenericRepository<TEntity>().Add(Record);
            await _data.SaveAsync();

            await AfterSaveActionsAsync();
            AfterSaveActions();

            return NavigateOnSuccess();
        }

        protected virtual Task AfterSaveActionsAsync() {
            return Task.CompletedTask;
        }
        protected virtual void AfterSaveActions() { }

        protected virtual Task BeforeSaveActionsAsync() {
            return Task.CompletedTask;
        }
        protected virtual void BeforeSaveActions() { }

        protected virtual IActionResult NavigateOnSuccess() {
            return RedirectToPage("./Index");
        }

        protected virtual IActionResult NavigationOnFailure() {
            return Page();
        }
    }
}
