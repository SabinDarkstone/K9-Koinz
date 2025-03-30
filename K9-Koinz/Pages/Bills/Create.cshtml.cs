using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Pages.Bills {
    public class CreateModel : CreatePageModel<Bill> {
        public SelectList GoalOptions { get; set; } = default!;

        public CreateModel(BillRepository repository, IDropdownPopulatorService dropdownService)
            : base(repository, dropdownService) { }

        protected override async Task OnPageLoadActionsAsync() {
            GoalOptions = await _dropdownService.GetSavingsGoalsAsync(null);
        }
    }
}
