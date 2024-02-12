using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Pages.Meta {
    public abstract class AbstractCreateModel<T> : PageModel where T : BaseEntity {
        protected readonly KoinzContext _context;
        protected readonly IAccountService _accountService;
        protected readonly IAutocompleteService _autocompleteService;
        protected readonly ITagService _tagService;

        [BindProperty]
        public T Record { get; set; } = default!;

        protected Guid? RelatedId { get; private set; }

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        protected AbstractCreateModel(KoinzContext context, IAccountService accountService, IAutocompleteService autocompleteService, ITagService tagService) {
            _context = context;
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
            AccountOptions = _accountService.GetAccountList(true);
            TagOptions = _tagService.GetTagList();
        }

        public virtual async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return NavigationOnFailure();
            }

            await BeforeSaveActions();

            _context.Set<T>().Add(Record);
            await _context.SaveChangesAsync();

            await AfterSaveActions();

            return NavigateOnSuccess();
        }

        protected abstract Task AfterSaveActions();
        protected abstract Task BeforeSaveActions();

        public IActionResult NavigateOnSuccess() {
            return RedirectToPage("./Index");
        }

        public IActionResult NavigationOnFailure() {
            return Page();
        }
    }
}
