using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.Savings.Goals {
    public class EditModel : EditPageModel<SavingsGoal> {
        public EditModel(SavingsRepository repository, IDropdownPopulatorService dropdownService)
            : base(repository, dropdownService) { }

        protected override IActionResult HandleNavigate(DbSaveResult saveResult) {
            if (saveResult.IsSuccess) {
                return RedirectToPage(PagePaths.SavingsIndex, new { view = "goals" });
            } else {
                return Page();
            }
        }
    }
}
