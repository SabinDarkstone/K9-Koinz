using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Budgets {
    public class CreateModel : AbstractCreateModel<Budget> {
        private readonly IBudgetService _budgetService;

        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger, IDropdownPopulatorService dropdownService, IBudgetService budgetService)
                : base(context, logger, dropdownService) {
            _budgetService = budgetService;
        }

        protected override async Task AfterSaveActionsAsync() {
            if (Record.DoNotUseCategories) {
                BudgetLine allowanceLine;
                var allCategory = await _context.Categories.SingleOrDefaultAsync(cat => cat.CategoryType == CategoryType.ALL);
                if (allCategory == null) {
                    var newAllCategory = new Category {
                        Name = "All",
                        ParentCategoryId = null,
                        CategoryType = CategoryType.ALL
                    };
                    _context.Categories.Add(newAllCategory);
                    await _context.SaveChangesAsync();
                    allCategory = newAllCategory;
                }
                allowanceLine = new BudgetLine {
                    BudgetId = Record.Id,
                    BudgetedAmount = Record.BudgetedAmount.Value,
                    BudgetCategoryId = allCategory.Id,
                    DoRollover = Record.DoNoCategoryRollover
                };
                _context.BudgetLines.Add(allowanceLine);
                await _context.SaveChangesAsync();

                if (Record.DoNoCategoryRollover) {
                    await CreateFirstBudgetLinePeriod(allowanceLine);
                    await _context.SaveChangesAsync();
                }
            }
        }

        protected override async Task BeforeSaveActionsAsync() {
            if (Record.BudgetTagId == Guid.Empty) {
                Record.BudgetTagId = null;
            } else {
                var tag = await _context.Tags.FindAsync(Record.BudgetTagId);
                Record.BudgetTagName = tag.Name;
            }
        }

        private async Task CreateFirstBudgetLinePeriod(BudgetLine budgetLine) {
            var (startDate, endDate) = Record.Timespan.GetStartAndEndDate();
            var totalSpentSoFar = (await _budgetService.GetTransactionsForCurrentBudgetLinePeriodAsync(budgetLine, DateTime.Now))
                .Sum(trans => trans.Amount);

            var firstPeriod = new BudgetLinePeriod {
                BudgetLineId = budgetLine.Id,
                StartingAmount = 0,
                StartDate = startDate,
                EndDate = endDate,
                SpentAmount = totalSpentSoFar
            };

            _context.BudgetLinePeriods.Add(firstPeriod);
        }
    }
}
