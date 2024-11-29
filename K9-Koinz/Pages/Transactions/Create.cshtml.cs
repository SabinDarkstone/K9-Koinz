using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Factories;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transactions {
    public class CreateModel : CreatePageModel<Transaction> {

        public CreateModel(IDropdownPopulatorService dropdownService, TransactionRepository repository)
            : base(repository, dropdownService) { }

        protected override IActionResult HandleNavigate(DbSaveResult saveResult) {
            if (saveResult.BeforeStatus == TriggerStatus.DUPLICATE_FOUND) {
                return RedirectToPage(PagePaths.TransactionDuplicateFound, new { id = Record.Id });
            } else if (saveResult.AfterStatus == TriggerStatus.GO_SAVINGS) {
                return RedirectToPage(PagePaths.SavingsAllocate, new { relatedId = Record.Id });
            } else {
                var routeValues = Bakery.MakeRouteFromCookie(Request.Cookies["backToTransactions"]);
                return RedirectToPage(PagePaths.TransactionIndex, routeValues);
            }
        }
    }
}
