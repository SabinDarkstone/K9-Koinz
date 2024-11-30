using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services;
using K9_Koinz.Triggers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Meta {
    [Obsolete("Use EditPageModel with a repository instead")]
    public abstract class AbstractEditModel<T> : AbstractDbPage where T : BaseEntity {
        protected readonly IDropdownPopulatorService _dropdownService;

        protected ITrigger<T> trigger;

        [BindProperty]
        public T Record { get; set; } = default!;

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        protected AbstractEditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger) {
            _dropdownService = dropdownService;
        }

        protected virtual async Task BeforePageLoadActions() {
            AccountOptions = await _dropdownService.GetAccountListAsync();
            TagOptions = await _dropdownService.GetTagListAsync();
        }

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (!id.HasValue) {
                return NotFound();
            }

            await BeforePageLoadActions();

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
            T oldRecord = _context.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id == Record.Id);

            _logger.LogInformation("EDIT " + Record.GetType().Name + ": " + Record.ToJson());
            if (trigger != null) {
                trigger.OnBeforeUpdate(new List<T> { oldRecord }, new List<T> { Record });
            }

            await BeforeSaveActionsAsync();
            BeforeSaveActions();

            if (!ModelState.IsValid) {
                foreach (var key in ModelState.Keys) {
                    if (ModelState[key].Errors.Count > 0) {
                        foreach (var error in ModelState[key].Errors) {
                            _logger.LogError(key + ": " + error.ErrorMessage + " | " + Record.GetType().GetProperty(key).GetValue(Record));
                        }
                    }
                }
                await BeforePageLoadActions();
                return NavigationOnFailure();
            }

            _context.Attach(Record).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();

                if (trigger != null) {
                    trigger.OnAfterUpdate(new List<T> { oldRecord }, new List<T> { Record });
                }

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
