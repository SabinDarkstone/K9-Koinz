using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Pages.Meta {
    public abstract class AbstractEditModel<T> : PageModel where T : BaseEntity {
        protected readonly KoinzContext _context;
        protected readonly IAccountService _accountService;
        protected readonly IAutocompleteService _autocompleteService;
        protected readonly ITagService _tagService;

        [BindProperty]
        public T Record { get; set; } = default!;

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        protected AbstractEditModel(KoinzContext context, IAccountService accountService, IAutocompleteService autocompleteService, ITagService tagService) {
            _context = context;
            _accountService = accountService;
            _autocompleteService = autocompleteService;
            _tagService = tagService;
        }

        protected bool RecordExists(Guid id) {
            return _context.Set<T>().Any(e => e.Id == id);
        }
    }
}
