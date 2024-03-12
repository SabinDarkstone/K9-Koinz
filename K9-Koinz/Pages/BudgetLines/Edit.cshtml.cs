using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.BudgetLines {
    public class EditModel : AbstractEditModel<BudgetLine> {
        private readonly IBudgetService _budgetService;

        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IAccountService accountService, IAutocompleteService autocompleteService,
            ITagService tagService, IBudgetService budgetService)
                : base(context, logger, accountService, autocompleteService, tagService) {
            _budgetService = budgetService;
        }

        protected override async Task<BudgetLine> QueryRecordAsync(Guid id) {
            return await _context.BudgetLines
                .Include(line => line.BudgetCategory)
                .FirstAsync(line => line.Id == id);
        }

        protected override async Task BeforeSaveActionsAsync() {
            var oldRecord = await _context.BudgetLines
                .Include(line => line.BudgetCategory)
                .Include(line => line.Budget)
                .AsNoTracking()
                .FirstOrDefaultAsync(line => line.Id == Record.Id);

            Record.BudgetCategory = oldRecord.BudgetCategory;
            Record.Budget = oldRecord.Budget;

            if (!oldRecord.DoRollover && Record.DoRollover) {
                _budgetService.DeleteOldBudgetLinePeriods(Record);
                await CreateFirstBudgetLinePeriod();
            }

            if (oldRecord.DoRollover && !Record.DoRollover) {
                _budgetService.DeleteOldBudgetLinePeriods(Record);
            }

            Record.BudgetCategoryId = oldRecord.BudgetCategoryId;
            Record.BudgetId = oldRecord.BudgetId;
            Record.BudgetCategory = null;
            Record.Budget = null;

            var category = await _context.Categories.FindAsync(Record.BudgetCategoryId);
            var budget = await _context.Budgets.FindAsync(Record.BudgetId);

            Record.BudgetCategoryName = category.Name;
            Record.BudgetName = budget.Name;
        }

        protected override IActionResult NavigationOnSuccess() {
            return RedirectToPage(PagePaths.BudgetEdit, new { id = Record.BudgetId } );
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
    }
}
