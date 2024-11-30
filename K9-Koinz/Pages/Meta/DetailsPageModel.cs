using K9_Koinz.Data.Repositories.Meta;
using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace K9_Koinz.Pages.Meta {
    public class DetailsPageModel<TEntity> : PageModel where TEntity : BaseEntity {
        protected readonly IRepository<TEntity> _repository;

        public TEntity Record { get; set; } = default!;
        public string QueryParamNav { get; set; }

        public DetailsPageModel(IRepository<TEntity> repository) {
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] Guid? id, [FromBody] string queryParams) {
            if (!id.HasValue) {
                return NotFound();
            }
            var record = await QueryRecord(id.Value);
            if (record == null) {
                return NotFound();
            }
            Record = record;
            QueryParamNav = queryParams;

            AfterQueryActions();

            return Page();
        }

        protected virtual async Task<TEntity> QueryRecord(Guid id) {
            return await _repository.GetByIdAsync(id);
        }

        protected virtual void AfterQueryActions() { }
    }
}
