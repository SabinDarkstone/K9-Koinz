using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.Savings.Goals {
    [Authorize]
    public class DeleteModel : AbstractDeleteModel<SavingsGoal> {
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override IActionResult NavigateOnSuccess() {
            return RedirectToPage(PagePaths.SavingsIndex, new { view = "goals" });
        }
    }
}
