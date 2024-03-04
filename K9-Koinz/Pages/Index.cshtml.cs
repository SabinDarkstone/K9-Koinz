using Microsoft.AspNetCore.Mvc.RazorPages;
using K9_Koinz.Data;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages {

    public class IndexModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ISpendingGraphService _spendingGraph;
        private readonly IDbCleanupService _dbCleanupService;

        public IndexModel(KoinzContext context, ISpendingGraphService spendingGraph, IDbCleanupService cleanupService) {
            _context = context;
            _spendingGraph = spendingGraph;
            _dbCleanupService = cleanupService;
        }

        public string ThisMonthSpendingJson { get; set; }
        public string LastMonthSpendingJson { get; set; }
        public List<Account> Accounts { get; set; } = default!;

        public async Task OnGetAsync() {
            await _dbCleanupService.DateMigrateBillSchedules();

            var results = await _spendingGraph.CreateGraphData();
            ThisMonthSpendingJson = results[0];
            LastMonthSpendingJson = results[1];

            var accounts = await _context.Accounts
                .AsNoTracking()
                .ToListAsync();
            if (accounts.Count > 0) {
                this.Accounts = accounts;
            }
        }
    }
}
