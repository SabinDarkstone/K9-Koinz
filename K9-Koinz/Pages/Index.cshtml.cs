using Microsoft.AspNetCore.Mvc.RazorPages;
using K9_Koinz.Data;
using K9_Koinz.Services;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages {

    public class IndexModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ISpendingGraphService _spendingGraph;

        public IndexModel(KoinzContext context, ISpendingGraphService spendingGraph) {
            _context = context;
            _spendingGraph = spendingGraph;
        }

        public string ThisMonthSpendingJson { get; set; }
        public string LastMonthSpendingJson { get; set; }
        public string ThreeMonthAverageSpendingJson { get; set; }

        public List<Account> Accounts { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync() {
            var j = 0;
            var i = 1 / j;
            var results = await _spendingGraph.CreateGraphData();
            ThisMonthSpendingJson = results[0];
            LastMonthSpendingJson = results[1];
            ThreeMonthAverageSpendingJson = results[2];

            var accounts = await _context.Accounts
                .AsNoTracking()
                .ToListAsync();
            if (accounts.Count > 0) {
                this.Accounts = accounts;
            }

            return Page();
        }
    }
}
