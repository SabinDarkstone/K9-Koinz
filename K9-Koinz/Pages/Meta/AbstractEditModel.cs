using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace K9_Koinz.Pages.Meta {
    public abstract class AbstractEditModel<T> : AbstractDbPage where T : BaseEntity {
        protected readonly IAccountService _accountService;
        protected readonly IAutocompleteService _autocompleteService;
        protected readonly ITagService _tagService;

        [BindProperty]
        public T Record { get; set; } = default!;

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        protected AbstractEditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IAccountService accountService, IAutocompleteService autocompleteService,
            ITagService tagService)
                : base(context, logger) {
            _accountService = accountService;
            _autocompleteService = autocompleteService;
            _tagService = tagService;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (!id.HasValue) {
                return NotFound();
            }

            AccountOptions = await _accountService.GetAccountListAsync(true);
            TagOptions = await _tagService.GetTagListAsync();

            await BeforeQueryActionsAsync();
            BeforeQueryActions();

            var record = await QueryRecordAsync(id.Value);
            if (record == null) {
                return NotFound();
            }

            Record = record;
            await AfterQueryActionsAsync();
            AfterQueryActions();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return NavigationOnFailure();
            }

            await BeforeSaveActionsAsync();
            BeforeSaveActions();

            _context.Attach(Record).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
                await AfterSaveActionsAsync();
                AfterSaveActions();
            } catch (DbUpdateConcurrencyException) {
                if (!RecordExists(Record.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NavigationOnSuccess();
        }

        protected bool RecordExists(Guid id) {
            return _context.Set<T>().Any(e => e.Id == id);
        }

        protected virtual async Task<T> QueryRecordAsync(Guid id) {
            return await _context.Set<T>().FindAsync(id);
        }
        
        protected virtual void BeforeQueryActions() { }
        protected virtual Task BeforeQueryActionsAsync() {
            return Task.CompletedTask;
        }

        protected virtual Task AfterQueryActionsAsync() {
            return Task.CompletedTask;
        }

        protected virtual void AfterQueryActions() { }

        protected virtual IActionResult NavigationOnFailure() {
            return Page();
        }

        protected virtual IActionResult NavigationOnSuccess() {
            return RedirectToPage("./Index");
        }
        
        protected virtual Task BeforeSaveActionsAsync() {
            return Task.CompletedTask;
        }

        protected virtual void BeforeSaveActions() { }

        protected virtual Task AfterSaveActionsAsync() {
            return Task.CompletedTask;
        }

        protected virtual void AfterSaveActions() { }
    }
}
