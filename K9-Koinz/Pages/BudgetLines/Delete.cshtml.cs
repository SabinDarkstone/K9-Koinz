using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.BudgetLines {
    public class DeleteModel : AbstractDeleteModel<BudgetLine> {
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<BudgetLine> QueryRecordAsync(Guid id) {
            return await _context.BudgetLines
                .Include(line => line.BudgetCategory)
                .Include(line => line.Budget)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        protected override IActionResult NavigateOnSuccess() {
            return RedirectToPage("/Budgets/Edit", new { id = Record.BudgetId });
        }
    }
}
