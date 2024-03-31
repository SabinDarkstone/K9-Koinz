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
            SavingsGoalTransferTotal = _context.Transactions
                .AsNoTracking()
                .Where(trans => trans.SavingsGoalId != null)
                .Where(trans => trans.Date.Date >= startDate.Date && trans.Date.Date <= endDate.Date)
                .GetTotal(true);

            BillsTotal = SimulateBills(referenceDate, budget.Timespan);

            return View(this);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        private double SimulateBills(DateTime referenceDate, BudgetTimeSpan timespan) {
            var activeBills = _context.Bills
                .AsNoTracking()
                .Include(bill => bill.RepeatConfig)
                .AsEnumerable()
                .Where(bill => bill.RepeatConfig.IsActive)
                .ToList();

            var (startDate, endDate) = timespan.GetStartAndEndDate(referenceDate);
            var runningTotal = 0d;

            // Get bills that have already been paid
            for (var simDate = startDate; simDate <= endDate; simDate += TimeSpan.FromDays(1)) {
                var todaysBills = activeBills.Where(bill => bill.RepeatConfig.LastFiring.HasValue && bill.RepeatConfig.LastFiring.Value.Date == simDate.Date).ToList();
                foreach (var bill in todaysBills) {
                    runningTotal += bill.Amount;
                }
            }

            // Get bills that have yet to be paid
            for (var simDate = startDate; simDate <= endDate; simDate += TimeSpan.FromDays(1)) {
                var todaysBills = activeBills.Where(bill => bill.RepeatConfig.NextFiring.Value.Date == simDate.Date).ToList();
                foreach (var bill in todaysBills) {
                    runningTotal += bill.Amount;
                    bill.RepeatConfig.FireNow();
                }
            }

            return runningTotal * -1;
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
