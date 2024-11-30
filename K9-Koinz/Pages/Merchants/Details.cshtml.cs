using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Merchants {
    public class DetailsModel : DetailsPageModel<Merchant> {
        private readonly ITrendGraphService _trendGraphService;
        private readonly TransactionRepository _transactionRepository;

        public DetailsModel(MerchantRepository repository, ITrendGraphService trendGraphService, TransactionRepository transactionRepository) : base(repository) {
            _trendGraphService = trendGraphService;
            _transactionRepository = transactionRepository;
        }

        public bool ChartError { get; set; }
        public string SpendingHistory { get; set; }

        public List<Transaction> Transactions { get; set; }

        protected override void AfterQueryActions() {
            Transactions = _transactionRepository.GetRecentMerchantTransactions(Record.Id, 50).Result;

            SpendingHistory = _trendGraphService.CreateGraphData(
                predicate: trans => trans.MerchantId == Record.Id,
                hideSavingsSpending: true
            ).Result;

            if (SpendingHistory == null) {
                ChartError = true;
            }
        }
    }
}
