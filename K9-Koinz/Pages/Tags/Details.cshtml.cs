using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Tags {

    public class DetailsModel : AbstractDetailsModel<Tag> {
        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger, ITrendGraphService trendGraph)
            : base(context, logger) {
                _trendGraphService = trendGraph;
            }

        private readonly ITrendGraphService _trendGraphService;

        public bool ChartError { get; set; }
        public string SpendingHistory { get; set; }

        protected override async Task<Tag> QueryRecordAsync(Guid id) {
            return await _context.Tags
                .Include(tag => tag.Transactions)
                .FirstOrDefaultAsync(tag => tag.Id == id);
        }

        protected override async Task AdditionalActionsAsync() {
            SpendingHistory = await _trendGraphService.CreateGraphData(
                predicate: trans => trans.TagId == Record.Id,
                hideSavingsSpending: true
            );
            if (SpendingHistory == null) {
                ChartError = true;
            }
        }
    }
}
