using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

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

    public class IndexModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly BudgetPeriodUtils _budgetPeriodUtils;

        public IndexModel(KoinzContext context, ILogger<IndexModel> logger) {
            _context = context;
            _logger = logger;
            _budgetPeriodUtils = new BudgetPeriodUtils(context);
        }

        public IList<Budget> Budgets { get; set; } = default!;
        public Budget SelectedBudget { get; set; }

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

            Budgets = await _context.Budgets
                .Include(bud => bud.BudgetTag)
                .OrderBy(bud => bud.SortOrder)
                .AsNoTracking()
                .ToListAsync();

            SelectedBudget = GetBudgetDetails(selectedBudget);

            if (SelectedBudget == null) {
                return;
            }

            GenerateBudgetPeriodOptions();
            RetrieveAndHandleTransactions();
            foreach (var budgetLine in SelectedBudget.RolloverExpenses) {
                budgetLine.GetPeriods(BudgetPeriod);
            }
            UpdatePreviousPeriods();
            UpdateCurrentPeriods();
            _context.SaveChanges();
        }

        // TODO: This needs to be cleaned up
        private Budget GetBudgetDetails(string selectedBudget) {
            var budgetQuery = _context.Budgets
                .Include(bud => bud.BudgetLines)
                    .ThenInclude(line => line.BudgetCategory)
                        .ThenInclude(cat => cat.Transactions)
                .Include(bud => bud.BudgetLines)
                    .ThenInclude(line => line.BudgetCategory)
                        .ThenInclude(cat => cat.ChildCategories)
                            .ThenInclude(cCat => cCat.Transactions)
                .Include(bud => bud.BudgetLines)
                    .ThenInclude(line => line.Periods)
                .Include(bud => bud.BudgetTag)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(selectedBudget)) {
                return budgetQuery.FirstOrDefault(bud => bud.Id == Guid.Parse(selectedBudget));
            } else {
                return budgetQuery.FirstOrDefault();
            }
        }

        private void GenerateBudgetPeriodOptions() {
            if (SelectedBudget.Timespan == BudgetTimeSpan.MONTHLY) {
                for (var i = 0; i < 12; i++) {
                    var optionDate = DateTime.Now.AddMonths(i * -1);
                    PeriodOptions.Add(new BudgetPeriodOption {
                        Value = optionDate,
                        Text = optionDate.FormatShortMonthAndYear(),
                        IsSelected = optionDate.Date == BudgetPeriod.Date,
                        IsDisabled = !_context.Transactions.AsNoTracking().Any(trans => trans.Date >= optionDate.StartOfMonth() && trans.Date <= optionDate.EndOfMonth())
                    });
                }
            } else if (SelectedBudget.Timespan == BudgetTimeSpan.WEEKLY) {
                for (var i = 0; i < 7; i++) {
                    var optionDate = DateTime.Now.AddDays(i * -7);
                    var weekStartDate = SelectedBudget.Timespan.GetStartAndEndDate(optionDate).Item1;
                    PeriodOptions.Add(new BudgetPeriodOption {
                        Value = optionDate,
                        Text = i == 0 ? "This Week" : "Week of " + weekStartDate.Month + "/" + weekStartDate.Day,
                        IsSelected = optionDate.Date == BudgetPeriod.Date,
                        IsDisabled = !_context.Transactions.AsNoTracking().Any(trans => trans.Date >= optionDate.StartOfWeek() && trans.Date <= optionDate.EndOfWeek())
                    });
                }
            } else if (SelectedBudget.Timespan == BudgetTimeSpan.YEARLY) {
                for (var i = 0; i < 4; i++) {
                    var optionDate = DateTime.Now.AddYears(i * -1);
                    PeriodOptions.Add(new BudgetPeriodOption {
                        Value = optionDate,
                        Text = i == 0 ? "This Year" : optionDate.Year.ToString(),
                        IsSelected = optionDate.Date == BudgetPeriod.Date,
                        IsDisabled = !_context.Transactions.AsNoTracking().Any(trans => trans.Date >= optionDate.StartOfYear() && trans.Date <= optionDate.EndOfYear())
                    });
                }
            }

            PeriodOptions.Reverse();
        }

        private void RetrieveAndHandleTransactions() {
            // Get transactions for each budget line and write values to unmapped properties in the budget line model
            foreach (var category in SelectedBudget.BudgetLines) {
                _ = category.GetTransactions(BudgetPeriod, _context, _logger);
            }

            // If the current budget uses categories, determine unallocated spending
            if (!SelectedBudget.DoNotUseCategories) {
                var newBudgetLines = SelectedBudget.GetUnallocatedSpending(_context, BudgetPeriod);
                SelectedBudget.UnallocatedLines = newBudgetLines;
            }
        }

        private void UpdateCurrentPeriods() {
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
                budgetLine.CurrentPeriod.SpentAmount = -1 * _budgetPeriodUtils.GetTransactionsForCurrentBudgetLinePeriod(budgetLine, BudgetPeriod).Sum(trans => trans.Amount);
                periodsToUpdate.Add(budgetLine.CurrentPeriod);

                // Update the starting amount based on rollover from the last period
                if (budgetLine.PreviousPeriod != null) {
                    budgetLine.CurrentPeriod.StartingAmount = budgetLine.BudgetedAmount - budgetLine.PreviousPeriod.SpentAmount;
                    periodsToUpdate.Add(budgetLine.PreviousPeriod);
                }
            }

            // Untrack budget from DB change tracker
            periodsToUpdate.ForEach(per => {
                per.BudgetLine = null;
            });

            _context.BudgetLinePeriods.UpdateRange(periodsToUpdate);
        }

        private BudgetLinePeriod CreateNewCurrentPeriod(BudgetLine budgetLine) {
            var (startDate, endDate) = SelectedBudget.Timespan.GetStartAndEndDate();
            var newPeriod = new BudgetLinePeriod {
                BudgetLineId = budgetLine.Id,
                StartDate = startDate,
                EndDate = endDate
            };

            _context.BudgetLinePeriods.Add(newPeriod);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            return newPeriod;
        }

        private void UpdatePreviousPeriods() {
            var periodsToUpdate = new List<BudgetLinePeriod>();
            foreach (var budgetLine in SelectedBudget.RolloverExpenses) {
                if (budgetLine.PreviousPeriod != null) {
                    budgetLine.PreviousPeriod.SpentAmount = -1 * _budgetPeriodUtils.GetTransactionsForPreviousLinePeriod(budgetLine, BudgetPeriod).Sum(trans => trans.Amount);
                    budgetLine.PreviousPeriod.BudgetLine = null;
                    periodsToUpdate.Add(budgetLine.PreviousPeriod);
                }
            }

            periodsToUpdate.ForEach(per => {
                per.BudgetLine = null;
            });

            _context.BudgetLinePeriods.UpdateRange(periodsToUpdate);
        }
    }
}
