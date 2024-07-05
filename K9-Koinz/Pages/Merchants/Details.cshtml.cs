using Microsoft.EntityFrameworkCore;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Merchants {
    public class DetailsModel : AbstractDetailsModel<Merchant> {

        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger, ITrendGraphService trendGraph)
            : base(context, logger) {
                _trendGraphService = trendGraph;
            }

        private readonly ITrendGraphService _trendGraphService;

        public bool ChartError { get; set; }
        public string SpendingHistory { get; set; }

        public List<Transaction> Transactions { get; set; }

        protected override async Task AdditionalActionsAsync() {
            Transactions = await _context.Transactions
                .AsNoTracking()
                .Where(trans => trans.MerchantId == Record.Id)
                .OrderByDescending(trans => trans.Date)
                .Take(50)
                .ToListAsync();

            SpendingHistory = await _trendGraphService.CreateGraphData(
                predicate: trans => trans.MerchantId == Record.Id,
                hideSavingsSpending: true
            );
            if (SpendingHistory == null) {
                ChartError = true;
            }
        }
    }
}
