﻿using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Utils;
using NuGet.Protocol;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers;

namespace K9_Koinz.Pages.Transactions {
    public class EditModel : AbstractEditModel<Transaction> {
        public SelectList GoalOptions { get; set; } = default!;
        public SelectList BillOptions { get; set; } = default!;

        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger, dropdownService) {
            trigger = new TransactionTrigger(context, logger);
        }

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
