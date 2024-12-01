using K9_Koinz.Data.Repositories.Meta;
using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace K9_Koinz.Pages.Meta {
    public class IndexPageModel<TEntity> : PageModel where TEntity : BaseEntity {
        protected readonly IRepository<TEntity> _repository;

        public IList<TEntity> Records { get; set; } = default!;

        public IndexPageModel(IRepository<TEntity> repository) {
            _repository = repository;
        }
    }
}
