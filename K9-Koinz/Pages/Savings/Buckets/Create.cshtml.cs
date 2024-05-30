using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.Savings.Buckets {
    [Authorize]
    public class CreateModel : AbstractCreateModel<SavingsGoal> {
        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger, dropdownService) { }

        protected override void BeforeSaveActions() {
            var account = _context.Accounts.Find(Record.AccountId);
            Record.SavingsType = SavingsType.BUCKET;
            Record.AccountName = account.Name;
            Record.StartDate = DateTime.Today;
            Record.IsActive = true;

            Record.SavedAmount = 0d;
        }

        protected override IActionResult NavigateOnSuccess() {
            return RedirectToPage(PagePaths.SavingsIndex, new { view = "buckets" });
        }
    }
}
