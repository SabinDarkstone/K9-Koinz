using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Utils;
using NuGet.Protocol;
using K9_Koinz.Triggers;

namespace K9_Koinz.Pages.Transactions {
    public class DeleteModel : AbstractDeleteModel<Transaction> {
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) {
            trigger = new TransactionTrigger(context, logger);
        }

        protected override async Task<Transaction> QueryRecordAsync(Guid id) {
            return await _context.Transactions
                .Include(trans => trans.Tag)
                .Include(trans => trans.Transfer)
                    .ThenInclude(fer => fer.Transactions)
                .Include(trans => trans.SplitTransactions)
                .FirstOrDefaultAsync(trans => trans.Id == id);
        }

        protected override IActionResult NavigateOnSuccess() {
            var transactionFilterCookie = Request.Cookies["backToTransactions"].FromJson<TransactionNavPayload>();
            return RedirectToPage(PagePaths.TransactionIndex, routeValues: new {
                sortOrder = transactionFilterCookie.SortOrder,
                catFilter = transactionFilterCookie.CatFilter,
                pageIndex = transactionFilterCookie.PageIndex,
                accountFilter = transactionFilterCookie.AccountFilter,
                minDate = transactionFilterCookie.MinDate,
                maxDate = transactionFilterCookie.MaxDate,
                merchFilter = transactionFilterCookie.MerchFilter
            });
        }
    }
}
