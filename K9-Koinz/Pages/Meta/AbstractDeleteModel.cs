using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Meta {
    public abstract class AbstractDeleteModel<TEntity> : AbstractDbPage where TEntity : BaseEntity {
        protected AbstractDeleteModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        [BindProperty]
        public TEntity Record { get; set; }
        public string ErrorMessage { get; set; }

        public virtual async Task<IActionResult> OnGetAsync(Guid? id, bool? saveChangedError = false) {
            if (!id.HasValue) {
                return NotFound();
            }
            var record = await QueryRecordAsync(id.Value);
            
            if (record == null) {
                return NotFound();
            }

            Record = record;

            await AfterQueryActionAsync();
            AfterQueryActions();

            if (saveChangedError.GetValueOrDefault()) {
                ErrorMessage = string.Format("Delete {0} failed. Try again.", id.Value.ToString());
            }

            return Page();
        }

        public virtual async Task<IActionResult> OnPostAsync(Guid? id) {
            if (!id.HasValue) {
                return NotFound();
            }

            var record = await _data.GetGenericRepository<TEntity>().GetByIdAsync(id);
            if (record == null) {
                return NotFound();
            }

            Record = record;
            try {
                await BeforeDeleteActionsAsync();
                BeforeDeleteActions();

                _data.GetGenericRepository<TEntity>().Remove(record);

                await AdditionalDatabaseActionsAsync();
                AdditioanlDatabaseActions();

                await _data.SaveAsync();
                return NavigateOnSuccess();
            } catch (DbUpdateException ex) {
                _logger.LogError(ex, ErrorMessage);
                return RedirectToPage("./Delete", new { id, saveChangedError = true });
            }
        }

        protected virtual IActionResult NavigateOnSuccess() {
            return RedirectToPage("./Index");
        }

        protected virtual async Task<TEntity> QueryRecordAsync(Guid id) {
            return await _data.GetGenericRepository<TEntity>().GetByIdAsync(id);
        }

        protected virtual Task AfterQueryActionAsync() {
            return Task.CompletedTask;
        }
        protected virtual void AfterQueryActions() { }

        protected virtual Task AdditionalDatabaseActionsAsync() {
            return Task.CompletedTask;
        }
        protected virtual void AdditioanlDatabaseActions() { }

        protected virtual void BeforeDeleteActions() { }

        protected virtual Task BeforeDeleteActionsAsync() {
            return Task.CompletedTask;
        }
    }
}
