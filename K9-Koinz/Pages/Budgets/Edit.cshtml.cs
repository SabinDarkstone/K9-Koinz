using K9_Koinz.Models;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Budgets {
    public class EditModel : EditPageModel<Budget> {
        public EditModel(BudgetRepository repository, IDropdownPopulatorService dropdownService)
            : base(repository, dropdownService) { }

        protected override async Task<Budget> QueryRecord(Guid id) {
            return await (_repository as BudgetRepository).GetBudgetDetails(id);
        }
    }
}
