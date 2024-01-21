using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Utils {
    public static class NamingUtils {

        public static void AssignNames(KoinzContext context) {
            AssignNamesToTransactions(context);
            AssignNamesToBudgetLines(context);
            AssignNamesToBudgets(context);
            AssignNamesToCategories(context);
        }

        private static void AssignNamesToTransactions(KoinzContext context) {
            if (context.Transactions.AsNoTracking().Any(trans => trans.AccountName == null || trans.CategoryName == null || trans.MerchantName == null)) {
                var transactions = context.Transactions
                    .Where(trans => trans.AccountName == null || trans.CategoryName == null || trans.MerchantName == null)
                    .Include(trans => trans.Category)
                    .Include(trans => trans.Account)
                    .Include(trans => trans.Merchant)
                    .ToList();

                foreach (var trans in transactions) {
                    trans.AccountName = trans.Account?.Name ?? string.Empty;
                    trans.CategoryName = trans.Category?.Name ?? string.Empty;
                    trans.MerchantName = trans.Merchant?.Name ?? string.Empty;
                }

                context.UpdateRange(transactions);
                context.SaveChanges();
            }
        }

        private static void AssignNamesToCategories(KoinzContext context) {
            if (context.Categories.Where(cat => cat.ParentCategoryId.HasValue).AsNoTracking().Any(cat => cat.ParentCategoryName == null)) {
                var categories = context.Categories
                    .Where(cat => cat.ParentCategoryName == null && cat.ParentCategoryId.HasValue)
                    .Include(cat => cat.ParentCategory)
                    .ToList();

                foreach (var cat in categories) {
                    cat.ParentCategoryName = cat.ParentCategory?.Name ?? string.Empty;
                }

                context.UpdateRange(categories);
                context.SaveChanges();
            }
        }

        private static void AssignNamesToBudgetLines(KoinzContext context) {
            if (context.BudgetLines.AsNoTracking().Any(line => line.BudgetName == null || line.BudgetCategoryName == null)) {
                var budgetLines = context.BudgetLines
                    .Where(line => line.BudgetCategoryName == null || line.BudgetName == null)
                    .Include(line => line.Budget)
                    .Include(line => line.BudgetCategory)
                    .ToList();

                foreach (var line in budgetLines) {
                    line.BudgetName = line.Budget?.Name ?? string.Empty;
                    line.BudgetCategoryName = line.BudgetCategory?.Name ?? string.Empty;
                }

                context.UpdateRange(budgetLines);
                context.SaveChanges();
            }
        }

        private static void AssignNamesToBudgets(KoinzContext context) {
            if (context.Budgets.AsNoTracking().Any(bud => bud.BudgetTagName == null && bud.BudgetTagId.HasValue)) {
                var budgets = context.Budgets
                    .Where(bud => bud.BudgetTagName == null && bud.BudgetTagId.HasValue)
                    .Include(bud => bud.BudgetTag)
                    .ToList();

                foreach (var budget in budgets) {
                    budget.BudgetTagName = budget.BudgetTag?.Name ?? string.Empty;
                }

                context.UpdateRange(budgets);
                context.SaveChanges();
            }
        }
    }
}
