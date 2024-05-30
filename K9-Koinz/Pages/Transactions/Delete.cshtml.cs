using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Utils;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authorization;

namespace K9_Koinz.Pages.Transactions {
    [Authorize]
    public class DeleteModel : AbstractDeleteModel<Transaction> {
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<Transaction> QueryRecordAsync(Guid id) {
            return await _context.Transactions
                .Include(trans => trans.Tag)
                .Include(trans => trans.Transfer)
                    .ThenInclude(fer => fer.Transactions)
                .Include(trans => trans.SplitTransactions)
                .FirstOrDefaultAsync(trans => trans.Id == id);
        }

        protected override void BeforeDeleteActions() {
            var splitTransactions = _context.Transactions
                .Where(trans => trans.ParentTransactionId == Record.Id);

            if (splitTransactions.Any()) {
                _context.Transactions.RemoveRange(splitTransactions);
            }
        }

        protected override void AdditioanlDatabaseActions() {
            if (Record.SavingsGoalId.HasValue) {
                var goal = _context.SavingsGoals.Find(Record.SavingsGoalId);
                goal.SavedAmount -= Record.Amount;
            }

            if (Record.TransferId.HasValue) {
                var otherTransaction = _context.Transactions
                    .Where(trans => trans.TransferId == Record.TransferId)
                    .Where(trans => trans.Id != Record.Id)
                    .SingleOrDefault();

                if (otherTransaction != null) {
                    _context.Transactions.Remove(otherTransaction);
                }

                var tranfer = _context.Transfers.Find(Record.TransferId);
                if (!tranfer.RepeatConfigId.HasValue) {
                    _context.Transfers.Remove(tranfer);
                }
            }
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
