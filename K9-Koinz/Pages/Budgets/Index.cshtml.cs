using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.Budgets
{
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(KoinzContext context, ILogger<IndexModel> logger) {
            _context = context;
            _logger = logger;
        }

        public IList<Budget> Budgets { get; set; } = default!;
        public Budget SelectedBudget { get; set; }

        public async Task<IActionResult> OnGetAsync(string selectedBudget) {
            Budgets = await _context.Budgets
                .Include(bud => bud.BudgetLines)
                    .ThenInclude(line => line.BudgetCategory)
                        .ThenInclude(cat => cat.Transactions)
                            .ThenInclude(trans => trans.Merchant)
                .Include(bud => bud.BudgetLines)
                    .ThenInclude(line => line.BudgetCategory)
                        .ThenInclude(cat => cat.ChildCategories)
                            .ThenInclude(cCat => cCat.Transactions)
                                .ThenInclude(trans => trans.Merchant)
                .OrderBy(bud => bud.SortOrder)
                .AsNoTracking()
                .ToListAsync();

            if (string.IsNullOrWhiteSpace(selectedBudget)) {
                SelectedBudget = Budgets.FirstOrDefault();
            } else {
                SelectedBudget = Budgets.FirstOrDefault(bud => bud.Id == Guid.Parse(selectedBudget), null);
            }

            if (SelectedBudget == null) {
                return Page();
            }

            foreach (var category in SelectedBudget.BudgetLines) {
                var transactions = category.GetTransactions();
            }

            var newBudgetLines = SelectedBudget.GetUnallocatedSpending(_context);
            SelectedBudget.UnallocatedLines = newBudgetLines;
            foreach (var line in newBudgetLines) {
                _logger.LogInformation(line.BudgetCategoryId.ToString() + " " + line.SpentAmount);
            }

            return Page();
        }
    }
}
