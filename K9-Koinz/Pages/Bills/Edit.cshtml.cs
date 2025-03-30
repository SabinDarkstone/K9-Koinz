using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Pages.Bills {
    public class EditModel : EditPageModel<Bill> {
        public SelectList GoalOptions { get; set; } = default!;

        public EditModel(BillRepository repository, IDropdownPopulatorService dropdownService)
            : base(repository, dropdownService) { }

        protected override async Task<Bill> QueryRecord(Guid id) {
            return await (_repository as BillRepository).GetBillWithDetails(id, true);
        }

        protected override async Task AfterQueryActions() {
            this.GoalOptions = await _dropdownService.GetSavingsGoalsAsync(null);
        }
    }
}
