using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Categories {
    public class DetailsModel : AbstractDetailsModel<Category> {
        public DetailsModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Category> QueryRecordAsync(Guid id) {
            return await _data.Categories.GetCategoryDetails(id);
        }
    }
}
