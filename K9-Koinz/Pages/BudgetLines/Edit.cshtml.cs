using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Humanizer.Localisation;

namespace K9_Koinz.Pages.BudgetLines {

    [DataContract]
    public class DataPoint {
        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "y")]
        public double? Y { get; set; }
    }

    public class EditModel : AbstractEditModel<BudgetLine> {
        private readonly IBudgetService _budgetService;

        public string SpendingHistory { get; set; }
        public string BudgetedAmount { get; set; }
        public bool ChartError { get; set; }

        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IAccountService accountService, ITagService tagService, IBudgetService budgetService)
                : base(context, logger, accountService, tagService) {
            _budgetService = budgetService;
        }

        protected override async Task<BudgetLine> QueryRecordAsync(Guid id) {
            var budgetLine = await _context.BudgetLines
                .Include(line => line.Budget)
                .Include(line => line.BudgetCategory)
                .FirstAsync(line => line.Id == id);

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
            return RedirectToPage(PagePaths.BudgetEdit, new { id = Record.BudgetId });
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

        private async Task<List<DataPoint>> GetSpendingHistory(BudgetLine line) {
            var transactions = await _context.Transactions
                .Include(trans => trans.Category)
                .AsNoTracking()
                .Where(trans => trans.CategoryId == line.BudgetCategoryId || trans.Category.ParentCategoryId == line.BudgetCategoryId)
                .Where(trans => !trans.IsSavingsSpending)
                .Where(trans => !trans.IsSplit)
                .Where(trans => trans.Date <= DateTime.Today.Date.Date && trans.Date.Date >= DateTime.Today.AddMonths(-11))
                .ToListAsync();

            transactions.ForEach(trans => _logger.LogInformation(trans.Id + " " + trans.Amount.ToString()));

            var groups = transactions
                .GroupBy(trans => trans.Date.Month + "|" + trans.Date.Year)
                .ToDictionary(grp => grp.Key, grp => grp.ToList().Sum(trans => trans.Amount));

            if (line.BudgetCategory.CategoryType == CategoryType.EXPENSE) {
                foreach (var key in groups.Keys) {
                    _logger.LogInformation(key.ToString());
                    groups[key] *= -1;
                }
            }

            var output = new List<DataPoint>();

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

                output.Add(new DataPoint {
                    Label = month + " '" + currentYear.ToString().Substring(2),
                    Y = amount
                });
            }

            return output;
        }
    }
}
