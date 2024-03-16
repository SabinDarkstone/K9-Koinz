using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Categories {
    public class EditModel : AbstractEditModel<Category> {
        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger, dropdownService) { }

        protected override async Task<Category> QueryRecordAsync(Guid id) {
            return await _context.Categories
                .Include(cat => cat.ChildCategories.OrderBy(cCat => cCat.Name))
                .Include(cat => cat.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        protected override async Task BeforeSaveActionsAsync() {
            var childCategories = await _context.Categories
                .Where(cat => cat.ParentCategoryId == Record.Id)
                .ToListAsync();
            foreach (var childCat in childCategories) {
                childCat.CategoryType = Record.CategoryType;
                _context.Attach(childCat).State = EntityState.Modified;
            }

            if (Record.ParentCategoryId.HasValue) {
                var parentCategory = await _context.Categories.FindAsync(Record.ParentCategoryId);
                Record.ParentCategoryName = parentCategory.Name;
            }
        }

        protected override async Task AfterSaveActionsAsync() {
            var childCategories = await _context.Categories
                .Where(cat => cat.ParentCategoryId == Record.Id)
                .ToListAsync();

            var relatedTransactions = await _context.Transactions
                .Where(trans => trans.CategoryId == Record.Id)
                .ToListAsync();

            var relatedBills = await _context.Bills
                .Where(bill => bill.CategoryId == Record.Id)
                .ToListAsync();

            var relatedBudgetLines = await _context.BudgetLines
                .Where(line => line.BudgetCategoryId == Record.Id)
                .ToListAsync();

            foreach (var cat in childCategories) {
                cat.ParentCategoryName = Record.Name;
            }

            foreach (var trans in relatedTransactions) {
                trans.CategoryName = Record.Name;
            }

            foreach (var bill in relatedBills) {
                bill.CategoryName = Record.Name;
            }

            foreach (var budgetLine in relatedBudgetLines) {
                budgetLine.BudgetCategoryName = Record.Name;
            }

            _context.Categories.UpdateRange(childCategories);
            _context.Transactions.UpdateRange(relatedTransactions);
            _context.Bills.UpdateRange(relatedBills);
            _context.BudgetLines.UpdateRange(relatedBudgetLines);
            await _context.SaveChangesAsync();
        }
    }
}
