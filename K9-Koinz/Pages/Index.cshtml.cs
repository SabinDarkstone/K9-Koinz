using Microsoft.AspNetCore.Mvc.RazorPages;
using K9_Koinz.Data;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages {

    public class IndexModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ISpendingGraphService _spendingGraph;
        private readonly IDbCleanupService _dbCleanupService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(KoinzContext context, ISpendingGraphService spendingGraph, IDbCleanupService cleanupService, IWebHostEnvironment environment, ILogger<IndexModel> logger) {
            _context = context;
            _spendingGraph = spendingGraph;
            _dbCleanupService = cleanupService;
            _environment = environment;
            _logger = logger;
        }

        public string ThisMonthSpendingJson { get; set; }
        public string LastMonthSpendingJson { get; set; }
        public string ThreeMonthAverageSpendingJson { get; set; }

        public List<Account> Accounts { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync() {
            // Remove transactions with an invalid date
            //var badTransactions = _context.Transactions
            //    .Where(trans => trans.Date.Date == DateTime.Parse("01/01/0001").Date)
            //    .ToList();
            //_context.RemoveRange(badTransactions);
            //await _context.SaveChangesAsync();

            // Add default icons to categories, as defined in JSON
            //var updatedCategories = CategoriesCreator.SetDefaultCategoryIcons(_environment, _context.Categories.ToList(), _logger);
            //_context.Categories.UpdateRange(updatedCategories);
            //await _context.SaveChangesAsync();

            //await _dbCleanupService.DateMigrateBillSchedules();
            //await _dbCleanupService.InstantiateRecurringTransfers();
            //await _dbCleanupService.FixTransferDates();

            var results = await _spendingGraph.CreateGraphData();
            ThisMonthSpendingJson = results[0];
            LastMonthSpendingJson = results[1];
            ThreeMonthAverageSpendingJson = results[2];

            _logger.LogInformation(ThisMonthSpendingJson);
            _logger.LogInformation(LastMonthSpendingJson);
            _logger.LogInformation(ThreeMonthAverageSpendingJson);

            var accounts = await _context.Accounts
                .AsNoTracking()
                .ToListAsync();
            if (accounts.Count > 0) {
                this.Accounts = accounts;
            }

            var configsToUpdate = new List<RepeatConfig>();
            foreach (var rptCfg in _context.RepeatConfigs.Where(rpt => rpt.NextFiring == null)) {
                rptCfg.NextFiring = rptCfg.CalculatedNextFiring;
                configsToUpdate.Add(rptCfg);
            }
            _context.UpdateRange(configsToUpdate);
            _context.SaveChanges();

            return Page();
        }
    }
}
