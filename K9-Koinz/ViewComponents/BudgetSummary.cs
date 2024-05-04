using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

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
        public async Task<IViewComponentResult> InvokeAsync(Budget budget, DateTime referenceDate) {
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

            SavingsGoalTransferTotal = SimulateSavingsGoals(referenceDate, budget.Timespan);
            BillsTotal = SimulateBills(referenceDate, budget.Timespan);

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
                var todaysBills = activeBills.Where(bill => bill.RepeatConfig.CalculatedNextFiring.Value.Date == simDate.Date).ToList();
                foreach (var bill in todaysBills) {
                    runningTotal -= bill.Amount;
                    bill.RepeatConfig.FireNow();
                }
            }

            return runningTotal;
        }

        private double SimulateSavingsGoals(DateTime referenceDate, BudgetTimeSpan timespan) {
            var activeSavingsTransfers = _context.Transfers
                .AsNoTracking()
                .Include(fer => fer.RepeatConfig)
                .Where(fer => fer.FromAccountId.HasValue)
                .Where(fer => fer.RepeatConfigId != null)
                .AsEnumerable()
                .Where(fer => fer.RepeatConfig.IsActive)
                .ToList();

            var (startDate, endDate) = timespan.GetStartAndEndDate(referenceDate);

            // Get savings goal transfer that have already happened
            var runningTotal = _context.Transactions
                .Where(trans => trans.SavingsGoalId != null)
                .Where(trans => trans.Amount > 0)
                .Where(trans => trans.Date.Date >= startDate.Date && trans.Date.Date <= endDate.Date)
                .Sum(trans => trans.Amount) * -1;

            // Get savings goal transfers that are scheduled to happen
            for (var simDate = startDate.Date; simDate <= endDate.Date; simDate += TimeSpan.FromDays(1)) {
                var todaysTransfers = activeSavingsTransfers.Where(fer => fer.RepeatConfig.CalculatedNextFiring.Value.Date == simDate.Date).ToList();
                foreach (var transfer in todaysTransfers) {
                    runningTotal -= transfer.Amount;
                    transfer.RepeatConfig.FireNow();
                }
            }

            return runningTotal;
        }


        [DisplayName("Budgeted Income")]
        public double BudgetedIncome { get; set; }
        [DisplayName("Extra Income")]
        public double ExtraIncome { get; set; }

        [DisplayName("Budgeted Expenses")]
        public double AllocatedExpenseTotal { get; set; }

        [DisplayName("Extra Expenses")]
        public double ExtraExpenseTotal { get; set; }

        [DisplayName("Savings Goals")]
        public double SavingsGoalTransferTotal { get; set; }
        [DisplayName("Planned Bills")]
        public double BillsTotal { get; set; }

        [DisplayName("Net Remaining")]
        public double NetAmount {
            get {
                return BudgetedIncome + ExtraIncome + AllocatedExpenseTotal + ExtraExpenseTotal + SavingsGoalTransferTotal + BillsTotal;
            }
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BudgetTimeSpan Timespan { get; set; }

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
