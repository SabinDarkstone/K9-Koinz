using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using Newtonsoft.Json;
using K9_Koinz.Models.Meta;
using K9_Koinz.Models.Helpers;

namespace K9_Koinz.Pages.BudgetLines {

    public class EditModel : AbstractEditModel<BudgetLine> {
        private readonly IBudgetService _budgetService;

        public string SpendingHistory { get; set; }
        public string BudgetedAmount { get; set; }
        public bool ChartError { get; set; }

        public EditModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService, IBudgetService budgetService)
                : base(data, logger, dropdownService) {
            _budgetService = budgetService;
        }

        protected override async Task<BudgetLine> QueryRecordAsync(Guid id) {
            var budgetLine = await _data.BudgetLineRepository.GetByIdAsync(id);

            try {
                if (budgetLine.Budget.Timespan == BudgetTimeSpan.MONTHLY) {
                    var spending = await GetSpendingHistory(budgetLine);
                    if (spending != null) {
                        SpendingHistory = JsonConvert.SerializeObject(spending);
                        ChartError = false;
                    }
                }
            } catch (Exception ex) {
                ChartError = true;
                _logger.LogError(JsonConvert.SerializeObject(ex));
            }

            return budgetLine;
        }

        protected override async Task BeforeSaveActionsAsync() {
            var oldRecord = await _data.BudgetLineRepository.GetByIdAsync(Record.Id);

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

            var category = await _data.CategoryRepository.GetByIdAsync(Record.BudgetCategoryId); ;
            var budget = await _data.BudgetRepository.GetByIdAsync(Record.BudgetId);

            Record.BudgetCategoryName = category.Name;
            Record.BudgetName = budget.Name;
        }

        protected override IActionResult NavigationOnSuccess() {
            return RedirectToPage(PagePaths.BudgetEdit, new { id = Record.BudgetId });
        }

        private async Task CreateFirstBudgetLinePeriod() {
            var parentBudget = await _data.BudgetRepository.GetByIdAsync(Record.BudgetId);
            var (startDate, endDate) = parentBudget.Timespan.GetStartAndEndDate();
            var totalSpentSoFar = (await _budgetService.GetTransactionsForCurrentBudgetLinePeriodAsync(Record, DateTime.Now))
                .GetTotal();

            var firstPeriod = new BudgetLinePeriod {
                BudgetLineId = Record.Id,
                StartingAmount = 0,
                StartDate = startDate,
                EndDate = endDate,
                SpentAmount = totalSpentSoFar
            };

            _data.BudgetLinePeriodRepository.Add(firstPeriod);
        }

        private async Task<List<GraphDataPoint>> GetSpendingHistory(BudgetLine line) {
            var transactions = await _data.TransactionRepository.GetForSpendingHistory(line.BudgetCategoryId);

            var groups = transactions
                .GroupBy(trans => trans.Date.Month + "|" + trans.Date.Year)
                .ToDictionary(grp => grp.Key, grp => grp.ToList().GetTotal());

            if (line.BudgetCategory.CategoryType == CategoryType.EXPENSE) {
                foreach (var key in groups.Keys) {
                    groups[key] *= -1;
                }
            }

            var output = new List<GraphDataPoint>();

            var startingKey = DateTime.Today.AddMonths(-11).Month + "|" + DateTime.Today.AddMonths(-11).Year;
            for (var i = 1; i < 12; i++) {
                var currentDate = DateTime.Today.AddMonths(-11 + i);
                var currentYear = currentDate.Year;
                var currentMonth = currentDate.Month;

                var amount = 0d;
                var month = DateUtils.GetMonthName(currentMonth);

                if (groups.ContainsKey(currentMonth + "|" + currentYear)) {
                    amount = groups[currentMonth + "|" + currentYear];
                }

                output.Add(new GraphDataPoint {
                    Label = month + " '" + currentYear.ToString().Substring(2),
                    Y = amount
                });
            }

            return output;
        }
    }
}
