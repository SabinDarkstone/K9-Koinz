using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Tags {

    public class DetailsModel : DetailsPageModel<Tag> {
        private readonly ITrendGraphService _trendGraphService;

        public DetailsModel(TagRepository repository, ITrendGraphService trendGraphService) : base(repository) {
            _trendGraphService = trendGraphService;
        }

        public bool ChartError { get; set; }
        public string SpendingHistory { get; set; }

        protected override async Task<Tag> QueryRecord(Guid id) {
            return await (_repository as TagRepository).GetTagAndTransactionsById(id);
        }

        protected override void AfterQueryActions() {
            SpendingHistory = _trendGraphService.CreateGraphData(
                predicate: trans => trans.TagId == Record.Id,
                hideSavingsSpending: true
            ).Result;

            if (SpendingHistory == null) {
                ChartError = true;
            }
        }
    }
}
