using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
            IncomeTotal = budget.IncomeLines
                .Sum(line => line.BudgetedAmount);
            AllocatedExpenseTotal = budget.ExpenseLines
                .Sum(line => line.BudgetedAmount) * -1;
            ExtraExpenseTotal = budget.UnallocatedExpenses
                .SelectMany(line => line.Transactions)
                .Sum(trans => trans.Amount);
            SavingsGoalTransferTotal = _context.Transactions
                .AsNoTracking()
                .Where(trans => trans.SavingsGoalId != null)
                .Where(trans => trans.Date.Date >= startDate.Date && trans.Date.Date <= endDate.Date)
                .Sum(trans => trans.Amount) * -1;

            return View(this);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        [DisplayName("Estimated Income")]
        public double IncomeTotal { get; set; }

        [DisplayName("Budgeted Expenses")]
        public double AllocatedExpenseTotal { get; set; }

        [DisplayName("Extra Expenses")]
        public double ExtraExpenseTotal { get; set; }

        [DisplayName("Savings Goals")]
        public double SavingsGoalTransferTotal { get; set; }

        [DisplayName("Net Remaining")]
        public double NetAmount {
            get {
                return IncomeTotal + AllocatedExpenseTotal + ExtraExpenseTotal + SavingsGoalTransferTotal;
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
