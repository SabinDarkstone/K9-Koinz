using K9_Koinz.Models;

namespace K9_Koinz.Utils {
    public static class BudgetExtensions {

        private static void CheckLines(Budget budget) {
            if (budget.BudgetLines == null) {
                throw new Exception("Budget must be retrieved with budget lines.");
            }
        }

        public static ICollection<BudgetLine> GetIncomeLines(this Budget budget) {
            CheckLines(budget);

            return budget.BudgetLines
                .Where(line => line.BudgetCategory.CategoryType == CategoryType.INCOME)
                .OrderBy(line => line.BudgetCategoryName)
                .ToList();
        }

        public static ICollection<BudgetLine> GetExpenseLines(this Budget budget) {
            CheckLines(budget);

            return budget.BudgetLines
                .Where(line => line.BudgetCategory.CategoryType == CategoryType.EXPENSE)
                .OrderBy(line => line.BudgetCategoryName)
                .ToList();
        }
    }
}
