using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Categories {

    public class DetailsModel : DetailsPageModel<Category> {

        private readonly ITrendGraphService _trendGraphService;

        public bool ChartError { get; set; }
        public string SpendingHistory { get; set; }

        public DetailsModel(CategoryRepository repository, ITrendGraphService trendGraphService) : base(repository) {
            _trendGraphService = trendGraphService;
        }

        protected override async Task<Category> QueryRecord(Guid id) {
            return await (_repository as CategoryRepository).GetCategoryWithFamily(id);
        }

        protected override void AfterQueryActions() {
            SpendingHistory = _trendGraphService.CreateGraphData(
                predicate: trans => trans.CategoryId == Record.Id || trans.Category.ParentCategoryId == Record.Id,
                hideSavingsSpending: true
            ).Result;
            if (SpendingHistory == null) {
                ChartError = true;
            }
        }
    }
}
