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
                    PeriodOptions.Add(new BudgetPeriodOption {
                        Value = optionDate,
                        Text = i == 0 ? "This Week" : i == 1 ? "1 Week Ago" : i + " Weeks Ago",
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
            foreach (var category in SelectedBudget.BudgetLines) {
                category.GetTransactions(BudgetPeriod);

            }

            var newBudgetLines = SelectedBudget.GetUnallocatedSpending(_context, BudgetPeriod);
            SelectedBudget.UnallocatedLines = newBudgetLines;
        }

        private void UpdateCurrentPeriods() {
            var periodsToUpdate = new List<BudgetLinePeriod>();
            foreach (var budgetLine in SelectedBudget.RolloverExpenses) {
                if (budgetLine.PreviousPeriod != null && budgetLine.CurrentPeriod == null) {
                    budgetLine.CurrentPeriod = CreateNewCurrentPeriod(budgetLine);
                }

                //if (budgetLine.CurrentPeriod == null) {
                //    continue;
                //}

                budgetLine.CurrentPeriod.SpentAmount = -1 * _budgetPeriodUtils.GetTransactionsForCurrentBudgetLinePeriod(budgetLine, BudgetPeriod).Sum(trans => trans.Amount);
                periodsToUpdate.Add(budgetLine.CurrentPeriod);
                if (budgetLine.PreviousPeriod != null) {
                    budgetLine.CurrentPeriod.StartingAmount = budgetLine.BudgetedAmount - budgetLine.PreviousPeriod.SpentAmount;
                    periodsToUpdate.Add(budgetLine.PreviousPeriod);
                }
            }

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
