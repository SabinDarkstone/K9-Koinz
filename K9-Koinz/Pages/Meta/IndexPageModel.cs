using K9_Koinz.Data.Repositories.Meta;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace K9_Koinz.Pages.Meta {
    public class IndexPageModel<TEntity> : PageModel where TEntity : BaseEntity {
        protected readonly IRepository<TEntity> _repository;

        public PaginatedList<TEntity> Records { get; set; } = default!;

        public IndexPageModel(IRepository<TEntity> repository) {
            _repository = repository;
        }
    }
}
