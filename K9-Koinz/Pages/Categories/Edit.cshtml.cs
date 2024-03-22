using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Categories {
    public class EditModel : AbstractEditModel<Category> {
        public EditModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(data, logger, dropdownService) { }

        protected override async Task<Category> QueryRecordAsync(Guid id) {
            return await _data.Categories.GetCategoryDetails(id);
        }

        protected override async Task BeforeSaveActionsAsync() {
            var childCategories = await _data.Categories.GetChildrenAsync(Record.Id);

            foreach (var childCat in childCategories) {
                childCat.CategoryType = Record.CategoryType;
            }
            _data.Categories.Update(childCategories);

            if (Record.ParentCategoryId.HasValue) {
                var parentCategory = await _data.Categories.GetByIdAsync(Record.ParentCategoryId);
                Record.ParentCategoryName = parentCategory.Name;
            }
        }

        protected override async Task AfterSaveActionsAsync() {
            var childCategories = await _data.Categories.GetChildrenAsync(Record.ParentCategoryId.Value);
            var relatedTransactions = await _data.Transactions.GetByCategory(Record.Id);
            var relatedBills = await _data.Bills.GetByCategory(Record.Id);
            var relatedBudgetLines = await _data.BudgetLines.GetByCategory(Record.Id);

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

            _data.Categories.Update(childCategories);
            _data.Transactions.Update(relatedTransactions);
            _data.Bills.Update(relatedBills);
            _data.BudgetLines.Update(relatedBudgetLines);
            await _data.SaveAsync();
        }
    }
}
