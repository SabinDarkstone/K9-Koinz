using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.BudgetLines {
    public class CreateModel : AbstractCreateModel<BudgetLine> {
        private readonly IBudgetService _budgetService;

        public CreateModel(KoinzContext context, IAccountService accountService,
            IAutocompleteService autocompleteService, ITagService tagService,
            IBudgetService budgetService)
                : base(context, accountService, autocompleteService, tagService) {
            _budgetService = budgetService;
        }

        public Budget Budget { get; set; }

        protected override async Task BeforePageLoadActions() {
            await base.BeforePageLoadActions();
            Budget = await _context.Budgets.FindAsync(RelatedId);
        }

        public IActionResult OnGetCategoryAutoComplete(string text) {
            return _autocompleteService.AutocompleteCategories(text.Trim());
        }

        private async Task CreateFirstBudgetLinePeriod() {
            var parentBudget = await _context.Budgets.FindAsync(Record.BudgetId);
            var (startDate, endDate) = parentBudget.Timespan.GetStartAndEndDate();
            var totalSpentSoFar = (await _budgetService.GetTransactionsForCurrentBudgetLinePeriod(Record, DateTime.Now)).Sum(trans => trans.Amount);

            var firstPeriod = new BudgetLinePeriod {
                BudgetLineId = Record.Id,
                StartingAmount = 0,
                StartDate = startDate,
                EndDate = endDate,
                SpentAmount = totalSpentSoFar
            };

            _context.BudgetLinePeriods.Add(firstPeriod);
        }

        protected override async Task AfterSaveActions() {
            if (Record.DoRollover) {
                await CreateFirstBudgetLinePeriod();
                await _context.SaveChangesAsync();
            }
        }

        protected override async Task BeforeSaveActions() {
            var category = await _context.Categories.SingleOrDefaultAsync(cat => cat.Id == Record.BudgetCategoryId);
            var budget = await _context.Budgets.SingleOrDefaultAsync(bud => bud.Id == Record.BudgetId);
            
            Record.BudgetCategoryName = category.Name;
            Record.BudgetName = budget.Name;
        }
    }
}
