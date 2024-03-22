using K9_Koinz.Data;
using K9_Koinz.Services;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages {

    public class IndexModel : AbstractDbPage {
        private readonly ISpendingGraphService _spendingGraph;

        public IndexModel(IRepositoryWrapper data, ISpendingGraphService spendingGraph, ILogger<IndexModel> logger)
            : base(data, logger) {
            _spendingGraph = spendingGraph;
            _logger = logger;
        }

        public string ThisMonthSpendingJson { get; set; }
        public string LastMonthSpendingJson { get; set; }
        public IEnumerable<Account> Accounts { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync() {
            var results = await _spendingGraph.CreateGraphData();
            ThisMonthSpendingJson = results[0];
            LastMonthSpendingJson = results[1];

            var accounts = await _data.AccountRepository.GetAll();
            if (accounts.Count() > 0) {
                this.Accounts = accounts;
            }

            return Page();
        }
    }
}
