using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.BudgetLines {
    public class CreateModel : AbstractCreateModel<BudgetLine> {
        private readonly IBudgetService _budgetService;

        public CreateModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService, IBudgetService budgetService)
                : base(data, logger, dropdownService) {
            _budgetService = budgetService;
        }

        public Budget Budget { get; set; }

        protected override async Task BeforePageLoadActions() {
            await base.BeforePageLoadActions();
            Budget = await _context.Budgets.FindAsync(RelatedId);
        }

        private async Task CreateFirstBudgetLinePeriod() {
            var parentBudget = await _context.Budgets.FindAsync(Record.BudgetId);
            var (startDate, endDate) = parentBudget.Timespan.GetStartAndEndDate();
            var totalSpentSoFar = (await _budgetService.GetTransactionsForCurrentBudgetLinePeriodAsync(Record, DateTime.Now)).GetTotal();

            var firstPeriod = new BudgetLinePeriod {
                BudgetLineId = Record.Id,
                StartingAmount = 0,
                StartDate = startDate,
                EndDate = endDate,
                SpentAmount = totalSpentSoFar
            };

            _context.BudgetLinePeriods.Add(firstPeriod);
        }

        protected override async Task AfterSaveActionsAsync() {
            if (Record.DoRollover) {
                await CreateFirstBudgetLinePeriod();
                await _context.SaveChangesAsync();
            }
        }

        protected override IActionResult NavigateOnSuccess() {
            return RedirectToPage(PagePaths.BudgetEdit, new { id = Record.BudgetId });
        }

        protected override async Task BeforeSaveActionsAsync() {
            var category = await _context.Categories.FindAsync(Record.BudgetCategoryId);
            var budget = await _context.Budgets.FindAsync(Record.BudgetId);
            
            Record.BudgetCategoryName = category.Name;
            Record.BudgetName = budget.Name;
        }
    }
}
