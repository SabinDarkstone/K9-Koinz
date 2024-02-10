using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace K9_Koinz.Utils {
    public static class BudgetUtils {
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
