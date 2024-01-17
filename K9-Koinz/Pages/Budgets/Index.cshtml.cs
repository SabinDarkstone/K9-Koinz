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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Pages.Budgets {
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(KoinzContext context, ILogger<IndexModel> logger) {
            _context = context;
            _logger = logger;
        }

        public IList<Budget> Budgets { get; set; } = default!;
        public Budget SelectedBudget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BudgetPeriod { get; set; } = DateTime.Now;

        public string BudgetDateString {
            get {
                return BudgetPeriod.FormatForUrl();
            }
        }

        public async Task OnGetAsync(string selectedBudget, DateTime? budgetPeriod) {
            if (budgetPeriod == null) {
                BudgetPeriod = DateTime.Now;
            } else {
                BudgetPeriod = budgetPeriod.Value;
            }

            Budgets = await _context.Budgets
                .OrderBy(bud => bud.SortOrder)
                .ToListAsync();

            SelectedBudget = await GetBudgetDetails(selectedBudget);

            if (SelectedBudget == null) {
                return;
            }

            RetrieveAndHandleTransactions();
        }

        private async Task<Budget> GetBudgetDetails(string selectedBudget) {
            var budgetQuery = _context.Budgets
                .Include(bud => bud.BudgetLines)
                .ThenInclude(line => line.BudgetCategory)
                    .ThenInclude(cat => cat.Transactions)
                        .ThenInclude(trans => trans.Merchant)
                .Include(bud => bud.BudgetLines)
                    .ThenInclude(line => line.BudgetCategory)
                        .ThenInclude(cat => cat.ChildCategories)
                            .ThenInclude(cCat => cCat.Transactions)
                                .ThenInclude(trans => trans.Merchant)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(selectedBudget)) {
                return await budgetQuery.FirstOrDefaultAsync(bud => bud.Id == Guid.Parse(selectedBudget));
            } else {
                return await budgetQuery.FirstOrDefaultAsync();
            }
        }

        private void RetrieveAndHandleTransactions() {
            foreach (var category in SelectedBudget.BudgetLines) {
                category.GetTransactions(BudgetPeriod);
            }

            var newBudgetLines = SelectedBudget.GetUnallocatedSpending(_context, BudgetPeriod);
            SelectedBudget.UnallocatedLines = newBudgetLines;
        }
    }
}
