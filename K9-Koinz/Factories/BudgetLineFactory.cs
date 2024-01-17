using K9_Koinz.Models;

namespace K9_Koinz.Factories {
    public static class BudgetLineFactory {
        public static BudgetLine NewBudgetLine(Guid budgetId, Guid categoryId, int amount) {
            return new BudgetLine { BudgetId = budgetId, BudgetCategoryId = categoryId, BudgetedAmount = amount };
        }
    }
}
