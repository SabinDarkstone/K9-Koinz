using K9_Koinz.Models;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Budgets {
    public class CreateModel : CreatePageModel<Budget> {
        public CreateModel(BudgetRepository repository, IDropdownPopulatorService dropdownService)
            : base(repository, dropdownService) { }
    }
}
