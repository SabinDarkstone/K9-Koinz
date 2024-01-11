using K9_Koinz.Models;

namespace K9_Koinz.Factories {
    public static class BudgetFactory {
        public static Budget NewWeeklyBudget(string name, string description) {
            return new Budget { Name = name, Description = description, Timespan = BudgetTimeSpan.WEEKLY };
        }

        public static Budget NewMonthlyBudget(string name, string description) {
            return new Budget { Name = name, Description = description, Timespan = BudgetTimeSpan.MONTHLY };
        }

        public static Budget NewYearlyBudget(string name, string description) {
            return new Budget { Name = name, Description = description, Timespan = BudgetTimeSpan.YEARLY };
        }
    }
}
