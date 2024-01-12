﻿using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Utils {
    public static class BudgetUtils {
        public static List<Transaction> GetTransactions(this BudgetLine line) {
            var (startDate, endDate) = line.Budget.Timespan.GetStartAndEndDate();
            var transactions = line.BudgetCategory.Transactions.Where(trans => trans.Date >= startDate && trans.Date <= endDate).ToList();
            var childCategoryTransactions = line.BudgetCategory.ChildCategories.SelectMany(cat => cat.Transactions.Where(trans => trans.Date >= startDate && trans.Date <= endDate)).ToList();
            transactions = transactions.Concat(childCategoryTransactions).ToList();
            line.SpentAmount = transactions.Sum(trans => trans.Amount);
            if (line.LineType == BudgetLineType.EXPENSE) {
                line.SpentAmount *= -1;
            }
            line.Transactions = transactions;
            return transactions;
        }

        public static List<BudgetLine> GetUnallocatedSpending(this Budget budget, KoinzContext context) {
            var categoryData = context.Categories.ToDictionary(cat => cat.Id);
            var allocatedCategories = budget.ExpenseLines.Select(line => line.BudgetCategoryId).Concat(budget.IncomeLines.Select(line => line.BudgetCategoryId)).ToList();
            var (startDate, endDate) = budget.Timespan.GetStartAndEndDate();
            var transactions = context.Transactions
                .Where(trans => trans.Date >= startDate && trans.Date <= endDate && !allocatedCategories.Contains(trans.CategoryId) &&
                    trans.Account.Type != AccountType.LOAN && trans.Account.Type != AccountType.INVESTMENT && trans.Account.Type != AccountType.PROPERTY)
                .ToList();

            var unallocatedBudgetLines = new Dictionary<Guid, BudgetLine>();
            foreach (var trans in transactions) {
                if (unallocatedBudgetLines.ContainsKey(trans.CategoryId)) {
                    unallocatedBudgetLines[trans.CategoryId].Transactions.Add(trans);
                } else {
                    var budgetCategory = categoryData[trans.CategoryId];
                    if (budgetCategory.ParentCategoryId.HasValue) {
                        budgetCategory.ParentCategory = categoryData[budgetCategory.ParentCategoryId.Value];
                    }

                    var newBudgetLine = new BudgetLine {
                        BudgetCategoryId = trans.CategoryId,
                        BudgetCategory = categoryData[trans.CategoryId],
                        BudgetId = budget.Id,
                        LineType = BudgetLineType.UNALLOCATED,
                        Transactions = new List<Transaction>()
                    };
                    newBudgetLine.Transactions.Add(trans);
                    unallocatedBudgetLines.Add(trans.CategoryId, newBudgetLine);
                }
            }

            foreach (var line in unallocatedBudgetLines.Values) {
                line.SpentAmount = line.Transactions.Sum(trans => trans.Amount);
            }

            return unallocatedBudgetLines.Values.ToList();
        }

        public static List<BudgetLine> SortCategories(this List<BudgetLine> lines) {
            var topLevelCategories = lines.Where(line => line.IsTopLevelCategory)
                .OrderBy(line => line.BudgetCategory.Name)
                .ToList();

            var sortedList = new List<BudgetLine>();
            foreach (var category in topLevelCategories) {
                sortedList.Add(category);
                var children = lines.Where(line => line.BudgetCategory.ParentCategoryId == category.BudgetCategoryId)
                    .OrderBy(line => line.BudgetCategory.Name)
                    .ToList();
                foreach (var childCategory in children) {
                    sortedList.Add(childCategory);
                }
            }

            foreach (var category in lines) {
                if (!sortedList.Contains(category)) {
                    sortedList.Add(category);
                }
            }

            return sortedList;
        }
    }
}
