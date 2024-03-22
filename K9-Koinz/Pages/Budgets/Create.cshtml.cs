using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Pages.Budgets {
    public class CreateModel : AbstractCreateModel<Budget> {
        private readonly IBudgetService _budgetService;

        public CreateModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger, IDropdownPopulatorService dropdownService, IBudgetService budgetService)
                : base(data, logger, dropdownService) {
            _budgetService = budgetService;
        }

        protected override async Task AfterSaveActionsAsync() {
            if (Record.DoNotUseCategories) {
                BudgetLine allowanceLine;
                var allCategory = _data.Categories.GetByName("All");
                if (allCategory == null) {
                    var newAllCategory = new Category {
                        Name = "All",
                        ParentCategoryId = null,
                        CategoryType = CategoryType.ALL
                    };

                    _data.Categories.Add(newAllCategory);
                    await _data.SaveAsync();
                    allCategory = newAllCategory;
                }
                allowanceLine = new BudgetLine {
                    BudgetId = Record.Id,
                    BudgetedAmount = Record.BudgetedAmount.Value,
                    BudgetCategoryId = allCategory.Id,
                    DoRollover = Record.DoNoCategoryRollover
                };
                _data.BudgetLines.Add(allowanceLine);
                await _data.SaveAsync();

                if (Record.DoNoCategoryRollover) {
                    await CreateFirstBudgetLinePeriod(allowanceLine);
                    await _data.SaveAsync();
                }
            }
        }

        protected override async Task BeforeSaveActionsAsync() {
            if (Record.BudgetTagId == Guid.Empty) {
                Record.BudgetTagId = null;
            } else {
                var tag = await _data.Tags.GetByIdAsync(Record.BudgetTagId);
                Record.BudgetTagName = tag.Name;
            }
        }

        private async Task CreateFirstBudgetLinePeriod(BudgetLine budgetLine) {
            var (startDate, endDate) = Record.Timespan.GetStartAndEndDate();
            var totalSpentSoFar = (await _budgetService.GetTransactionsForCurrentBudgetLinePeriodAsync(budgetLine, DateTime.Now))
                .GetTotal();

            var firstPeriod = new BudgetLinePeriod {
                BudgetLineId = budgetLine.Id,
                StartingAmount = 0,
                StartDate = startDate,
                EndDate = endDate,
                SpentAmount = totalSpentSoFar
            };

            _data.BudgetLinePeriods.Add(firstPeriod);
        }
    }
}
