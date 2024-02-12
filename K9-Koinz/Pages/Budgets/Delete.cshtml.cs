using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Budgets {
    public class DeleteModel : AbstractDeleteModel<Budget> {
        public List<BudgetLine> BudgetLines { get; set; }
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public override async Task<IActionResult> OnGetAsync(Guid? id, bool? saveChangedError = false) {
            if (id == null) {
                return NotFound();
            }

            var budget = await _context.Budgets.FirstOrDefaultAsync(m => m.Id == id);

            if (budget == null) {
                return NotFound();
            } else {
                Record = budget;
            }

            var unsortedLines = await _context.BudgetLines
                .Where(line => line.BudgetId == Record.Id)
                .ToListAsync();

            BudgetLines = unsortedLines.SortCategories();

            if (Record.DoNotUseCategories) {
                Record.BudgetedAmount = BudgetLines.First().BudgetedAmount;
            }

            return Page();
        }
    }
}
