using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Pages.Transfers.Recurring {
    public class EditModel : AbstractEditModel<Transfer> {
        public EditModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger, IDropdownPopulatorService dropdownService)
            : base(data, logger, dropdownService) { }

        public SelectList GoalOptions { get; set; } = default!;

        protected override async Task<Transfer> QueryRecordAsync(Guid id) {
            return await _data.Transfers.GetDetails(id);
        }

        protected override void AfterQueryActions() {
            GoalOptions = _data.SavingsGoals.GetForDropdown(Record.ToAccountId);
        }

        protected override void BeforeSaveActions() {
            if (Record.TagId == Guid.Empty) {
                Record.TagId = null;
            }

            if (Record.SavingsGoalId == Guid.Empty) {
                Record.SavingsGoalId = null;
            }
        }

        protected override IActionResult NavigationOnSuccess() {
            return RedirectToPage(PagePaths.TransferManage);
        }
    }
}
