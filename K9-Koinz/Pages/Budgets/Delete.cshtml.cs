using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.Budgets {
    public class DeleteModel : AbstractDeleteModel<Budget> {
        public List<BudgetLine> BudgetLines { get; set; }
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task AfterQueryActionAsync() {
            var unsortedLines = await _context.BudgetLines
                .Where(line => line.BudgetId == Record.Id)
                .ToListAsync();

            BudgetLines = unsortedLines.SortCategories();

            if (Record.DoNotUseCategories) {
                Record.BudgetedAmount = BudgetLines.First().BudgetedAmount;
            }
        }
    }
}
