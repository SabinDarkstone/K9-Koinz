using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.ViewComponents {
    [ViewComponent(Name = "BudgetSummary")]
    public class BudgetSummary : ViewComponent {
        private readonly KoinzContext _context;
        private ILogger<BudgetSummary> _logger;

        public BudgetSummary(KoinzContext context, ILogger<BudgetSummary> logger) {
            _context = context;
            _logger = logger;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IViewComponentResult> InvokeAsync(Budget budget, DateTime referenceDate, bool current) {
            var (startDate, endDate) = budget.Timespan.GetStartAndEndDate(referenceDate);
            BudgetedIncome = budget.IncomeLines
                .Sum(line => line.BudgetedAmount);
            ExtraIncome = budget.UnallocatedIncomes
                .SelectMany(line => line.Transactions)
                .GetTotal();
            AllocatedExpenseTotal = budget.ExpenseLines
                .Sum(line => line.BudgetedAmount) * -1;
            ExtraExpenseTotal = budget.UnallocatedExpenses
                .SelectMany(line => line.Transactions)
                .GetTotal();
            CurrentExpensesTotal = budget.ExpenseLines
                .Sum(line => line.SpentAmount) * -1;

            UseCurrentExpenses = current;
            BudgetPeriod = referenceDate;

            SavingsGoalTransferTotal = SimulateSavingsGoals(referenceDate, budget.Timespan);
            BillsTotal = SimulateBills(referenceDate, budget.Timespan);
            TotalRollover = CalculateRollover(budget);

            // Used for transaction modals
            StartDate = startDate;
            EndDate = endDate;
            Timespan = budget.Timespan;

            return View(this);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        private double SimulateBills(DateTime referenceDate, BudgetTimeSpan timespan) {
            var activeBills = _context.Bills
                .AsNoTracking()
                .Include(bill => bill.RepeatConfig)
                .Where(bill => bill.IsActive)
                .AsEnumerable()
                .Where(bill => bill.RepeatConfig.IsActive)
                .ToList();

            var (startDate, endDate) = timespan.GetStartAndEndDate(referenceDate);

            // Get bills that have already been paid
            var runningTotal = _context.Transactions
                .Where(trans => trans.BillId.HasValue)
                .Where(trans => trans.Date.Date >= startDate.Date && trans.Date.Date < endDate.Date)
                .Sum(trans => trans.Amount);

            // Get bills that have yet to be paid
            for (var simDate = startDate.Date; simDate <= endDate.Date; simDate += TimeSpan.FromDays(1)) {
                var todaysBills = activeBills
                    .Where(bill => bill.RepeatConfig.IsActive)
                    .Where(bill => bill.RepeatConfig.CalculatedNextFiring.Value.Date == simDate.Date)
                    .ToList();
                foreach (var bill in todaysBills) {
                    runningTotal -= bill.Amount;
                    if (bill.IsRepeatBill) {
                        bill.RepeatConfig.FireNow();
                    }
                }
            }

            return runningTotal;
        }

        private double SimulateSavingsGoals(DateTime referenceDate, BudgetTimeSpan timespan) {
            var activeSavingsTransfers = _context.Transfers
                .AsNoTracking()
                .Include(fer => fer.RepeatConfig)
                .Where(fer => fer.FromAccountId.HasValue)  // Do not include income transfers
                .Where(fer => fer.RepeatConfigId != null)  // Only include scheduled transfers
                .AsEnumerable()
                .Where(fer => fer.RepeatConfig.IsActive)  // Ensure the transfers are active
                .ToList();

            var (startDate, endDate) = timespan.GetStartAndEndDate(referenceDate);

            // Get savings goal transfer that have already happened
            var savingsTransactions = _context.Transactions
                .Include(trans => trans.Transfer)
                    .ThenInclude(fer => fer.RecurringTransfer)
                .Where(trans => trans.SavingsGoalId != null)
                .Where(trans => trans.Amount > 0)
                .Where(trans => trans.Date.Date >= startDate.Date && trans.Date.Date <= endDate.Date)
                .ToList();

            for (var i = savingsTransactions.Count - 1; i >= 0; i--) {
                if (savingsTransactions[i].TransferId != null && savingsTransactions[i].Transfer.IsTransferFromBudget) {
                    savingsTransactions.RemoveAt(i);
                }
            }

            _logger.LogWarning("Existing Transactions");
            savingsTransactions.ForEach(x => _logger.LogInformation(x.Amount + " " + x.Id + " " + x.Date.Date));

            var runningTotal = savingsTransactions.Sum(trans => trans.Amount) * -1;

            _logger.LogWarning("Upcoming Transactions");
            // Get savings goal transfers that are scheduled to happen
            for (var simDate = startDate.Date; simDate <= endDate.Date; simDate += TimeSpan.FromDays(1)) {
                var todaysTransfers = activeSavingsTransfers.Where(fer => fer.RepeatConfig.CalculatedNextFiring.Value.Date == simDate.Date).ToList();
                todaysTransfers.ForEach(x => _logger.LogInformation(x.Amount + " " + x.Id + " " + x.Date.Date));
                foreach (var transfer in todaysTransfers) {
                    runningTotal -= transfer.Amount;
                    transfer.RepeatConfig.FireNow();
                }
            }

            return runningTotal;
        }

        private double CalculateRollover(Budget budget) {
            var totalRollover = 0d;
            foreach (var line in budget.ExpenseLines) {
                if (line.CurrentPeriod != null) {
                    totalRollover += line.CurrentPeriod.StartingAmount;
                }
            }

            return totalRollover;
        }


        [DisplayName("Budgeted Income")]
        public double BudgetedIncome { get; set; }
        [DisplayName("Extra Income")]
        public double ExtraIncome { get; set; }

        [DisplayName("Budgeted Expenses")]
        public double AllocatedExpenseTotal { get; set; }

        [DisplayName("Current Expenses")]
        public double CurrentExpensesTotal { get; set; }

        [DisplayName("Extra Expenses")]
        public double ExtraExpenseTotal { get; set; }

        [DisplayName("Budgeted Savings")]
        public double SavingsGoalTransferTotal { get; set; }
        [DisplayName("Planned Bills")]
        public double BillsTotal { get; set; }

        [DisplayName("Total Rollover")]
        public double TotalRollover { get; set; }

        [DisplayName("Net Remaining")]
        public double NetAmount {
            get {
                var total = BudgetedIncome + TotalRollover + ExtraIncome + ExtraExpenseTotal + SavingsGoalTransferTotal + BillsTotal;
                if (UseCurrentExpenses) {
                    total += CurrentExpensesTotal;
                } else {
                    total += AllocatedExpenseTotal;
                }
                return total;
            }
        }

        [DisplayName("Current")]
        public bool UseCurrentExpenses { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BudgetTimeSpan Timespan { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BudgetPeriod { get; set; }

        public string RefDateValue {
            get {
                return BudgetPeriod.FormatForUrl();
            }
        }

        public string AlertClasses {
            get {
                var output = "alert ";
                if (NetAmount >= 0) {
                    return output + "alert-success";
                } else {
                    return output + "alert-danger";
                }
            }
        }
    }
}
