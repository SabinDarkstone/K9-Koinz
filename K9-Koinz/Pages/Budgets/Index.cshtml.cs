using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using System.ComponentModel.DataAnnotations;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Budgets {

    public class BudgetPeriodOption {
        public DateTime Value { get; set; }
        public string ValueString {
            get {
                return Value.FormatForUrl();
            }
        }
        public string Text { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class IndexModel : AbstractDbPage {
        private readonly IBudgetService _budgetService;

        public IndexModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger, IBudgetService budgetService)
            : base(data, logger) {
            _budgetService = budgetService;
        }


        private Budget selectedBudget;
        public IEnumerable<Budget> Budgets { get; set; } = default!;
        public Budget SelectedBudget {
            get {
                if (selectedBudget == null) {
                    return null;
                }
                selectedBudget.BudgetLines = selectedBudget.BudgetLines.OrderBy(line => line.BudgetCategoryName).ToList();
                return selectedBudget;
            }
            set {
                selectedBudget = value;
            }
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BudgetPeriod = DateTime.Now;

        public List<BudgetPeriodOption> PeriodOptions { get; set; } = new();

        public async Task OnGetAsync(string selectedBudget, DateTime? budgetPeriod) {
            if (budgetPeriod.HasValue) {
                BudgetPeriod = budgetPeriod.Value;
            } else {
                BudgetPeriod = DateTime.Now;
            }

            Budgets = await _data.BudgetRepository.GetAllAsync();

            SelectedBudget = await _data.BudgetRepository.GetBudgetDetails(selectedBudget);
            if (SelectedBudget == null) {
                return;
            }

            GenerateBudgetPeriodOptions();
            await RetrieveAndHandleTransactionsAsync();
            foreach (var budgetLine in SelectedBudget.RolloverExpenses) {
                budgetLine.GetCurrentAndPreviousPeriods(BudgetPeriod);
            }
            await UpdatePreviousPeriodsAsync();
            await UpdateCurrentPeriodsAsync();
            await _data.SaveAsync();
        }

        private void GenerateBudgetPeriodOptions() {
            if (SelectedBudget.Timespan == BudgetTimeSpan.MONTHLY) {
                for (var i = 0; i < 12; i++) {
                    var optionDate = DateTime.Now.AddMonths(i * -1);
                    PeriodOptions.Add(new BudgetPeriodOption {
                        Value = optionDate,
                        Text = optionDate.FormatShortMonthAndYear(),
                        IsSelected = optionDate.Date == BudgetPeriod.Date,
                        IsDisabled = !_data.TransactionRepository.AnyInMonth(optionDate.Date)
                    });
                }
            } else if (SelectedBudget.Timespan == BudgetTimeSpan.WEEKLY) {
                for (var i = 0; i < 8; i++) {
                    var optionDate = DateTime.Now.AddDays(i * -7);
                    var weekStartDate = SelectedBudget.Timespan.GetStartAndEndDate(optionDate).Item1;
                    PeriodOptions.Add(new BudgetPeriodOption {
                        Value = optionDate,
                        Text = i == 0 ? "This Week" : "Week of " + weekStartDate.Month + "/" + weekStartDate.Day,
                        IsSelected = optionDate.Date == BudgetPeriod.Date,
                        IsDisabled = !_data.TransactionRepository.AnyInWeek(optionDate.Date)
                    });
                }
            } else if (SelectedBudget.Timespan == BudgetTimeSpan.YEARLY) {
                for (var i = 0; i < 4; i++) {
                    var optionDate = DateTime.Now.AddYears(i * -1);
                    PeriodOptions.Add(new BudgetPeriodOption {
                        Value = optionDate,
                        Text = i == 0 ? "This Year" : optionDate.Year.ToString(),
                        IsSelected = optionDate.Date == BudgetPeriod.Date,
                        IsDisabled = !_data.TransactionRepository.AnyInYear(optionDate.Date)
                    });
                }
            }

            PeriodOptions.FirstOrDefault().IsDisabled = false;
            PeriodOptions.Reverse();
        }

        private async Task RetrieveAndHandleTransactionsAsync() {
            // Get transactions for each budget line and write values to unmapped properties in the budget line model
            foreach (var budgetLine in SelectedBudget.BudgetLines) {
                _budgetService.GetTransactions(budgetLine, BudgetPeriod);
            }

            // If the current budget uses categories, determine unallocated spending
            if (!SelectedBudget.DoNotUseCategories) {
                var newBudgetLines = await _budgetService.GetUnallocatedSpendingAsync(SelectedBudget, BudgetPeriod);
                SelectedBudget.UnallocatedLines = newBudgetLines;
            }
        }

        private async Task UpdateCurrentPeriodsAsync() {
            var periodsToUpdate = new List<BudgetLinePeriod>();
            
            // Loop through each budget line with rollover enabled
            foreach (var budgetLine in SelectedBudget.RolloverExpenses) {
                // Check if the previous period exists, but the current period does not
                if (budgetLine.PreviousPeriod != null && budgetLine.CurrentPeriod == null) {
                    // If this happens, we entered a new period and need to create a new period in the DB
                    budgetLine.CurrentPeriod = CreateNewCurrentPeriod(budgetLine);
                } else if (budgetLine.PreviousPeriod == null && budgetLine.CurrentPeriod == null) {
                    // We are browswing a budget period BEFORE rollover was activated, so ignore rollover this time
                    continue;
                }

                // Set the spent amount for the period based on the sum of the amounts of transations.
                // Multiply by -1 to make value positive
                budgetLine.CurrentPeriod.SpentAmount = (await _budgetService.GetTransactionsForCurrentBudgetLinePeriodAsync(budgetLine, BudgetPeriod))
                    .GetTotal(true);
                periodsToUpdate.Add(budgetLine.CurrentPeriod);

                // Update the starting amount based on rollover from the last period
                if (budgetLine.PreviousPeriod != null) {
                    budgetLine.CurrentPeriod.StartingAmount = (budgetLine.BudgetedAmount + budgetLine.PreviousPeriod.StartingAmount) - budgetLine.PreviousPeriod.SpentAmount;
                    periodsToUpdate.Add(budgetLine.PreviousPeriod);
                }
            }

            // Untrack budget from DB change tracker
            periodsToUpdate.ForEach(per => {
                per.BudgetLine = null;
            });

            _data.BudgetLinePeriodRepository.Update(periodsToUpdate);
        }

        private BudgetLinePeriod CreateNewCurrentPeriod(BudgetLine budgetLine) {
            var (startDate, endDate) = SelectedBudget.Timespan.GetStartAndEndDate();
            var newPeriod = new BudgetLinePeriod {
                BudgetLineId = budgetLine.Id,
                StartDate = startDate,
                EndDate = endDate
            };

            _data.BudgetLinePeriodRepository.Add(newPeriod);
            _data.Save();

            return newPeriod;
        }

        private async Task UpdatePreviousPeriodsAsync() {
            var periodsToUpdate = new List<BudgetLinePeriod>();
            foreach (var budgetLine in SelectedBudget.RolloverExpenses) {
                if (budgetLine.PreviousPeriod == null) {
                    // Skip this line, there is no previous period
                    continue;
                }

                budgetLine.PreviousPeriod.SpentAmount = (await _budgetService.GetTransactionsForPreviousLinePeriodAsync(budgetLine, BudgetPeriod)).GetTotal(true);
                budgetLine.PreviousPeriod.BudgetLine = null;
                periodsToUpdate.Add(budgetLine.PreviousPeriod);
            }

            periodsToUpdate.ForEach(per => {
                per.BudgetLine = null;
            });

            _data.BudgetLinePeriodRepository.Update(periodsToUpdate);
        }
    }
}
