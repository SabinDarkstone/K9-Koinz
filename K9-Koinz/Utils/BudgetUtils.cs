﻿using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace K9_Koinz.Utils {
    public static class BudgetUtils {
        public static List<Transaction> GetTransactions(this BudgetLine line, DateTime period, KoinzContext context, ILogger<Pages.Budgets.IndexModel> logger) {
            var (startDate, endDate) = line.Budget.Timespan.GetStartAndEndDate(period);

            if (line.Budget.DoNotUseCategories) {
                line.BudgetCategory.Transactions = context.Transactions.Where(trans => trans.Category.CategoryType != CategoryType.TRANSFER).ToList();
            }

            var transactionsIQ = line.BudgetCategory.Transactions.Where(trans => trans.Date >= startDate && trans.Date <= endDate);

            if (line.Budget.BudgetTagId.HasValue) {
                transactionsIQ = transactionsIQ.Where(trans => trans.TagId == line.Budget.BudgetTagId.Value);
            }
            var transactions = transactionsIQ.AsEnumerable();

            if (line.BudgetCategory.CategoryType == CategoryType.INCOME || line.BudgetCategory.CategoryType == CategoryType.EXPENSE) {
                var childCategoryTransactionsIQ = line.BudgetCategory.ChildCategories.SelectMany(cat => cat.Transactions.Where(trans => trans.Date >= startDate && trans.Date <= endDate)).AsEnumerable();
                if (line.Budget.BudgetTagId.HasValue) {
                    childCategoryTransactionsIQ = childCategoryTransactionsIQ.Where(trans => trans.TagId == line.Budget.BudgetTagId);
                }
                var childCategoryTransactions = childCategoryTransactionsIQ.AsEnumerable();
                transactions = [.. transactions, .. childCategoryTransactions];
            }

            line.SpentAmount = transactions.Sum(trans => trans.Amount);
            if ((line.BudgetCategory.CategoryType == CategoryType.EXPENSE || line.BudgetCategory.CategoryType == CategoryType.ALL) && line.SpentAmount != 0.0) {
                line.SpentAmount *= -1;
            }
            line.Transactions = transactions.ToList();
            return transactions.ToList();
        }

        public static List<BudgetLine> GetUnallocatedSpending(this Budget budget, KoinzContext context, DateTime period) {
            var categoryData = context.Categories.AsNoTracking().ToDictionary(cat => cat.Id);

            // Is this budget not using categories?
            if (budget.BudgetLines == null || budget.BudgetLines.Count == 0) {
                return new List<BudgetLine>();
            }

            // Get all budget categories from income and expense lines
            var allocatedCategories = budget.ExpenseLines
                .Select(line => line.BudgetCategoryId)
                .Concat(budget.IncomeLines.Select(line => line.BudgetCategoryId));

            var transferCategoryIds = categoryData.Values.Where(cat => cat.CategoryType == CategoryType.TRANSFER).Select(cat => cat.Id);
            allocatedCategories = allocatedCategories.Concat(transferCategoryIds).AsEnumerable();

            // Get child categories for those allocated categories, too
            var allocatedChildCategories = budget.ExpenseLines.SelectMany(line => line.BudgetCategory.ChildCategories.Select(cat => cat.Id)).AsEnumerable().Concat(budget.IncomeLines.SelectMany(line => line.BudgetCategory.ChildCategories.Select(cat => cat.Id)).ToList());
            
            // Concat those two lists of Category GUIDs together
            var allUnallocatedCategories = allocatedCategories.Concat(allocatedChildCategories);

            var (startDate, endDate) = budget.Timespan.GetStartAndEndDate(period);
            var transactionsIQ = context.Transactions.AsNoTracking()
                .Where(trans => trans.Date >= startDate && trans.Date <= endDate && !allUnallocatedCategories.Contains(trans.CategoryId) &&
                    trans.Account.Type != AccountType.LOAN && trans.Account.Type != AccountType.INVESTMENT && trans.Account.Type != AccountType.PROPERTY);
            if (budget.BudgetTagId.HasValue) {
                transactionsIQ = transactionsIQ.Where(trans => trans.TagId ==  budget.BudgetTagId.Value);
            }
            var transactions = transactionsIQ.ToList();

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
                        Transactions = new List<Transaction>()
                    };
                    newBudgetLine.Transactions.Add(trans);
                    unallocatedBudgetLines.Add(trans.CategoryId, newBudgetLine);
                }
            }

            foreach (var line in unallocatedBudgetLines.Values) {
                line.SpentAmount = line.Transactions.Sum(trans => trans.Amount);
            }

            return unallocatedBudgetLines.Values.Where(line => line.Transactions.Sum(trans => trans.Amount) != 0).ToList();
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
