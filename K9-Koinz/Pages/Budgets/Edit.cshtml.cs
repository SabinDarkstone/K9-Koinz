using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Budgets {
    public class EditModel : AbstractEditModel<Budget> {
        private readonly IBudgetService _budgetService;

        private BudgetLine oldBudgetLineRecord;

        public EditModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService, IBudgetService budgetService)
                : base(data, logger, dropdownService) {
            _budgetService = budgetService;
        }

        protected override async Task<Budget> QueryRecordAsync(Guid id) {
            return await _data.Budgets.GetBudgetDetailsShorter(id);
        }

        protected override void AfterQueryActions() {
            if (Record.DoNotUseCategories) {
                Record.BudgetedAmount = Record.BudgetLines.First().BudgetedAmount;
                Record.DoNoCategoryRollover = Record.BudgetLines.First().DoRollover;
            }
        }

        protected override async Task BeforeSaveActionsAsync() {
            if (Record.BudgetTagId == Guid.Empty) {
                Record.BudgetTagId = null;
            } else {
                var tag = await _data.Tags.GetByIdAsync(Record.BudgetTagId);
                Record.BudgetTagName = tag.Name;
            }

            if (Record.DoNotUseCategories) {
                oldBudgetLineRecord = (await _data.BudgetLines
                    .GetByBudget(Record.Id)).SingleOrDefault();
            }
        }

        protected override void AfterSaveActions() {
            base.AfterSaveActions();
        }

        protected override async Task AfterSaveActionsAsync() {
            if (Record.DoNotUseCategories) {
                var allowanceLine = (await _data.BudgetLines.GetByBudget(Record.Id)).SingleOrDefault();
                allowanceLine.BudgetedAmount = Record.BudgetedAmount.Value;
                allowanceLine.DoRollover = Record.DoNoCategoryRollover;

                _data.BudgetLines.Update(allowanceLine);
                await _data.SaveAsync();

                if (!oldBudgetLineRecord.DoRollover && Record.DoNoCategoryRollover) {
                    _budgetService.DeleteOldBudgetLinePeriods(allowanceLine);
                    await CreateFirstBudgetLinePeriodAsync(allowanceLine);
                }

                if (!Record.DoNoCategoryRollover) {
                    _budgetService.DeleteOldBudgetLinePeriods(allowanceLine);
                }
            }
        }

        private async Task CreateFirstBudgetLinePeriodAsync(BudgetLine budgetLine) {
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
            await _data.SaveAsync();
        }
    }
}
