using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace K9_Koinz.ViewComponents {
    [ViewComponent(Name = "BudgetSummary")]
    public class BudgetSummary : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync(Budget budget) {
            IncomeTotal = budget.IncomeLines
                .Sum(line => line.BudgetedAmount);
            AllocatedExpenseTotal = budget.ExpenseLines
                .Sum(line => line.BudgetedAmount) * -1;
            ExtraExpenseTotal = budget.UnallocatedExpenses
                .SelectMany(line => line.Transactions)
                .Sum(trans => trans.Amount);

            return View(this);
        }

        [DisplayName("Estimated Income")]
        public double IncomeTotal { get; set; }

        [DisplayName("Budgeted Expenses")]
        public double AllocatedExpenseTotal { get; set; }

        [DisplayName("Extra Expenses")]
        public double ExtraExpenseTotal { get; set; }

        [DisplayName("Net Remaining")]
        public double NetAmount {
            get {
                return IncomeTotal + AllocatedExpenseTotal + ExtraExpenseTotal;
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
