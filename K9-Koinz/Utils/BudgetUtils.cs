using K9_Koinz.Models;

namespace K9_Koinz.Utils
{
    public static class BudgetUtils
    {
        public static List<Transaction> GetTransactions(this BudgetLine line)
        {
            var (startDate, endDate) = line.Budget.Timespan.GetStartAndEndDate();
            var transactions = line.BudgetCategory.Transactions.Where(trans => trans.Date >= startDate && trans.Date <= endDate).ToList();
            var childCategoryTransactions = line.BudgetCategory.ChildCategories.SelectMany(cat => cat.Transactions.Where(trans => trans.Date >= startDate && trans.Date <= endDate)).ToList();
            transactions = transactions.Concat(childCategoryTransactions).ToList();
            line.SpentAmount = transactions.Sum(trans => trans.Amount) * -1;
            line.Transactions = transactions;
            return transactions;
        }

        public static List<BudgetLine> SortCategories(this List<BudgetLine> lines)
        {
            var topLevelCategories = lines.Where(line => line.IsTopLevelCategory)
                .OrderBy(line => line.BudgetCategory.Name)
                .ToList();

            var sortedList = new List<BudgetLine>();
            foreach (var category in topLevelCategories)
            {
                sortedList.Add(category);
                var children = lines.Where(line => line.BudgetCategory.ParentCategoryId == category.BudgetCategoryId)
                    .OrderBy(line => line.BudgetCategory.Name)
                    .ToList();
                foreach (var childCategory in children)
                {
                    sortedList.Add(childCategory);
                }
            }

            foreach (var category in lines)
            {
                if (!sortedList.Contains(category))
                {
                    sortedList.Add(category);
                }
            }

            return sortedList;
        }
    }
}
