using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Categories {
    public class UpdateCategoryNames : IHandler<Category> {
        private readonly KoinzContext _context;

        public UpdateCategoryNames(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Category> oldList, List<Category> newList) {
            var updatedCategoryIds = newList.Select(cat => cat.Id).ToHashSet();

            var childCategories = _context.Categories
                .Where(cat => updatedCategoryIds.Contains(cat.ParentCategoryId.Value))
                .ToList();

            var transactions = _context.Transactions
                .Where(trans => updatedCategoryIds.Contains(trans.CategoryId.Value))
                .ToList();

            var bills = _context.Bills
                .Where(bill => updatedCategoryIds.Contains(bill.CategoryId.Value))
                .ToList();

            var budgetLines = _context.BudgetLines
                .Where(line => updatedCategoryIds.Contains(line.BudgetCategoryId))
                .ToList();

            foreach (var cat in childCategories) {
                cat.ParentCategoryName = newList.First(c => c.Id == cat.ParentCategoryId).Name;
            }

            foreach (var trans in transactions) {
                trans.CategoryName = newList.First(c => c.Id == trans.CategoryId).Name;
            }

            foreach (var bill in bills) {
                bill.CategoryName = newList.First(c => c.Id == bill.CategoryId).Name;
            }

            foreach (var line in budgetLines) {
                line.BudgetCategoryName = newList.First(c => c.Id == line.BudgetCategoryId).Name;
            }

            _context.Categories.UpdateRange(childCategories);
            _context.Transactions.UpdateRange(transactions);
            _context.Bills.UpdateRange(bills);
            _context.BudgetLines.UpdateRange(budgetLines);
        }
    }
}
