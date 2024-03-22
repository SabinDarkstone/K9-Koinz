using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Meta {
    public abstract class AbstractEditModel<TEntity> : AbstractDbPage where TEntity : BaseEntity {
        protected readonly IDropdownPopulatorService _dropdownService;

        [BindProperty]
        public TEntity Record { get; set; } = default!;

        public List<SelectListItem> AccountOptions;
        public SelectList TagOptions;

        protected AbstractEditModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(data, logger) {
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
            if (!ModelState.IsValid) {
                await BeforePageLoadActions();
                return NavigationOnFailure();
            }

            await BeforeSaveActionsAsync();
            BeforeSaveActions();

            _data.GetGenericRepository<TEntity>().Update(Record);

            try {
                await _data.SaveAsync();
                await AfterSaveActionsAsync();
                AfterSaveActions();
            } catch (DbUpdateConcurrencyException) {
                if (!await _data.GetGenericRepository<TEntity>().DoesExistAsync(Record.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NavigationOnSuccess();
        }

        protected virtual async Task<TEntity> QueryRecordAsync(Guid id) {
            return await _data.GetGenericRepository<TEntity>().GetByIdAsync(id);
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
