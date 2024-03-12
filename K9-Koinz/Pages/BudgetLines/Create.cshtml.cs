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

        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger, IAccountService accountService,
            IAutocompleteService autocompleteService, ITagService tagService, IBudgetService budgetService)
                : base(context, logger, accountService, autocompleteService, tagService) {
            _budgetService = budgetService;
        }

        public Budget Budget { get; set; }

        protected override async Task BeforePageLoadActions() {
            await base.BeforePageLoadActions();
            Budget = await _context.Budgets.FindAsync(RelatedId);
        }

        public async Task<IActionResult> OnGetCategoryAutoComplete(string text) {
            return await _autocompleteService.AutocompleteCategoriesAsync(text.Trim());
        }

        private async Task CreateFirstBudgetLinePeriod() {
            var parentBudget = await _context.Budgets.FindAsync(Record.BudgetId);
            var (startDate, endDate) = parentBudget.Timespan.GetStartAndEndDate();
            var totalSpentSoFar = (await _budgetService.GetTransactionsForCurrentBudgetLinePeriodAsync(Record, DateTime.Now)).Sum(trans => trans.Amount);

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
