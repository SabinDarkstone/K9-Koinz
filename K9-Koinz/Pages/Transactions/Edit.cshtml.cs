using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Utils;
using NuGet.Protocol;
using K9_Koinz.Models.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace K9_Koinz.Pages.Transactions {
    [Authorize]
    public class EditModel : AbstractEditModel<Transaction> {
        public SelectList GoalOptions { get; set; } = default!;
        public SelectList BillOptions { get; set; } = default!;

        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger, dropdownService) { }

        protected override async Task<Transaction> QueryRecordAsync(Guid id) {
            return await _context.Transactions
                .Include(trans => trans.Category)
                .Include(trans => trans.SplitTransactions)
                .SingleOrDefaultAsync(trans => trans.Id == id);
        }

        protected override async Task AfterQueryActionsAsync() {
            if (Record.Category.CategoryType == CategoryType.TRANSFER) {
                GoalOptions = new SelectList(await _context.SavingsGoals
                    .Where(goal => goal.AccountId == Record.AccountId)
                    .ToListAsync(), nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
            } else if (Record.IsSavingsSpending || Record.SavingsGoalId.HasValue) {
                GoalOptions = new SelectList(
                    await _context.SavingsGoals.ToListAsync(),
                    nameof(SavingsGoal.Id), nameof(SavingsGoal.Name)
                );
            }

            if (Record.Category.CategoryType == CategoryType.EXPENSE) {
                BillOptions = new SelectList(await _context.Bills
                    .Where(bill => bill.AccountId == Record.AccountId)
                    .OrderBy(bill => bill.Name)
                    .ToListAsync(), nameof(Bill.Id), nameof(Bill.Name));
            }
        }

        protected override async Task BeforeSaveActionsAsync() {
            var category = await _context.Categories.FindAsync(Record.CategoryId);
            var merchant = await _context.Merchants.FindAsync(Record.MerchantId);
            var account = await _context.Accounts.FindAsync(Record.AccountId);
            if (!Record.IsSplit) {
                Record.CategoryName = category.Name;
            } else {
                Record.CategoryName = "Multiple";
            }
            Record.MerchantName = merchant.Name;
            Record.AccountName = account.Name;

            if (Record.TagId == Guid.Empty) {
                Record.TagId = null;
            }
            if (Record.BillId == Guid.Empty) {
                Record.BillId = null;
            }
            if (Record.SavingsGoalId.HasValue) {
                if (Record.SavingsGoalId.Value == Guid.Empty) {
                    Record.SavingsGoalId = null;
                } else {
                    var savingsGoal = await _context.SavingsGoals.FindAsync(Record.SavingsGoalId);
                    Record.SavingsGoalName = savingsGoal.Name;
                }
            }

            if (Record.TransferId.HasValue) {
                var otherTransaction = await _context.Transactions
                    .Where(trans => trans.TransferId == Record.TransferId)
                    .Where(trans => trans.Id != Record.Id)
                    .SingleOrDefaultAsync();

                otherTransaction.Amount = -1 * Record.Amount;
                otherTransaction.Date = Record.Date;
                otherTransaction.Notes = Record.Notes;
                otherTransaction.TagId = Record.TagId;

                _context.Transactions.Update(otherTransaction);
            }

            if (Record.IsSplit) {
                var childTransactions = _context.Transactions
                    .Where(trans => trans.ParentTransactionId == Record.Id)
                    .ToList();

                if (childTransactions.All(splt => splt.MerchantId == childTransactions[0].MerchantId)) {
                    childTransactions.ForEach(splt => {
                        splt.MerchantId = Record.MerchantId;
                        splt.MerchantName = Record.MerchantName;
                    });

                    _context.Transactions.UpdateRange(childTransactions);
                }
            }
        }

        protected override IActionResult NavigationOnSuccess() {
            if (Record.IsSavingsSpending && !Record.SavingsGoalId.HasValue) {
                return RedirectToPage(PagePaths.SavingsAllocate, new { relatedId = Record.Id });
            }

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
