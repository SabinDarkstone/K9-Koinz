using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace K9_Koinz.Pages.Meta {
    public abstract class AbstractCreateModel<T> : AbstractDbPage where T : BaseEntity {
        protected readonly IAccountService _accountService;
        protected readonly IAutocompleteService _autocompleteService;
        protected readonly ITagService _tagService;

        [BindProperty]
        public T Record { get; set; } = default!;

        protected Guid? RelatedId { get; private set; }

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        protected AbstractCreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IAccountService accountService, IAutocompleteService autocompleteService,
            ITagService tagService) : base(context, logger) {
            _accountService = accountService;
            _autocompleteService = autocompleteService;
            _tagService = tagService;
        }

        public virtual async Task<IActionResult> OnGetAsync(Guid? relatedId) {
            RelatedId = relatedId;

            await BeforePageLoadActions();
            return Page();
        }

        protected virtual async Task BeforePageLoadActions() {
            AccountOptions = await _accountService.GetAccountListAsync(true);
            TagOptions = await _tagService.GetTagListAsync();
        }

        public virtual async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return NavigationOnFailure();
            }

            await BeforeSaveActionsAsync();
            BeforeSaveActions();

            _context.Set<T>().Add(Record);
            await _context.SaveChangesAsync();

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
