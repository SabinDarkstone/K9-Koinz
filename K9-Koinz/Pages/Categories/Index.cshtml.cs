using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Categories {
    public class IndexModel : AbstractIndexModel<Category> {
        public IndexModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public async Task OnGetAsync() {
            var catList = await _data.CategoryRepository.GetAll();

            RecordList = catList
                .Where(cat => !cat.IsChildCategory)
                .ToList();
        }
    }
}
