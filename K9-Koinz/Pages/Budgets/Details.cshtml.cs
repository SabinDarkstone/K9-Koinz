using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Budgets {
    public class DetailsModel : AbstractDetailsModel<Budget> {
        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public List<BudgetLine> BudgetLines { get; set; }

        protected override async Task AdditionalActionsAsync() {
            var unsortedLines = await _context.BudgetLines
                .Where(line => line.BudgetId == Record.Id)
                .Include(line => line.Periods)
                .ToListAsync();

            BudgetLines = unsortedLines.SortCategories();

            if (Record.DoNotUseCategories) {
                Record.BudgetedAmount = BudgetLines.First().BudgetedAmount;
            }
        }
    }
}
