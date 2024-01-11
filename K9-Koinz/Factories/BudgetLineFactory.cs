using K9_Koinz.Models;

namespace K9_Koinz.Factories {
    public static class BudgetLineFactory {
        public static BudgetLine NewIncomeLine(Guid budgetId, Guid categoryId, int amount) {
            return new BudgetLine { BudgetId = budgetId, BudgetCategoryId = categoryId, LineType = BudgetLineType.INCOME, BudgetedAmount = new decimal(amount) };
        }

        public static BudgetLine NewExpenseLine(Guid budgetId, Guid categoryId, int amount) {
            return new BudgetLine { BudgetId = budgetId, BudgetCategoryId = categoryId, LineType = BudgetLineType.EXPENSE, BudgetedAmount = new decimal(amount) };
        }
    }
}
