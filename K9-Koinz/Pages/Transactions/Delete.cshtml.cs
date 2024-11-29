using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Utils;
using K9_Koinz.Factories;

namespace K9_Koinz.Pages.Transactions {
    public class DeleteModel : DeletePageModel<Transaction> {

        public DeleteModel(TransactionRepository repository) : base(repository) { }

        protected override async Task<Transaction> QueryRecord(Guid id) {
            return await (_repository as TransactionRepository).GetTransactionWithDetailsById(id);
        }

        protected override IActionResult NavigateOnSuccess() {
            var routeValues = Bakery.MakeRouteFromCookie(Request.Cookies["backToTransactions"]);
            return RedirectToPage(PagePaths.TransactionIndex, routeValues);
        }
    }
}
