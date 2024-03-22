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
            return await _data.CategoryRepository.GetCategoryDetails(id);
        }

        protected override async Task BeforeSaveActionsAsync() {
            var childCategories = await _data.CategoryRepository.GetChildrenAsync(Record.Id);

            foreach (var childCat in childCategories) {
                childCat.CategoryType = Record.CategoryType;
            }
            _data.CategoryRepository.Update(childCategories);

            if (Record.ParentCategoryId.HasValue) {
                var parentCategory = await _data.CategoryRepository.GetByIdAsync(Record.ParentCategoryId);
                Record.ParentCategoryName = parentCategory.Name;
            }
        }

        protected override async Task AfterSaveActionsAsync() {
            var childCategories = await _data.CategoryRepository.GetChildrenAsync(Record.ParentCategoryId.Value);
            var relatedTransactions = await _data.TransactionRepository.GetByCategory(Record.Id);
            var relatedBills = await _data.BillRepository.GetByCategory(Record.Id);
            var relatedBudgetLines = await _data.BudgetLineRepository.GetByCategory(Record.Id);

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

            _data.CategoryRepository.Update(childCategories);
            _data.TransactionRepository.Update(relatedTransactions);
            _data.BillRepository.Update(relatedBills);
            _data.BudgetLineRepository.Update(relatedBudgetLines);
            await _data.SaveAsync();
        }
    }
}
