using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.Meta {
    public abstract class AbstractDetailsModel<T> : AbstractDbPage where T : BaseEntity {
        protected AbstractDetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public T Record { get; set; } = default!;
        public string QueryParamsNav { get; set; }

        public async Task<IActionResult> OnGetAsync([FromQuery] Guid? id, [FromBody] string queryParams) {
            if (!id.HasValue) {
                return NotFound();
            }

            var record = await QueryRecordAsync(id.Value);
            if (record == null) {
                return NotFound();
            }

            QueryParamsNav = queryParams;
            Record = record;

            await AdditionalActionsAsync();
            AdditionalActions();

            return Page();
        }

        protected virtual async Task<T> QueryRecordAsync(Guid id) {
            return await _context.Set<T>().FindAsync(id);
        }

        protected virtual Task AdditionalActionsAsync() {
            return Task.CompletedTask;
        }
        protected virtual void AdditionalActions() { }
    }
}
