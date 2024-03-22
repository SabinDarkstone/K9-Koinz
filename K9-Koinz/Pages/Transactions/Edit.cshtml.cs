using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.Transactions {
    public class EditModel : AbstractEditModel<Transaction> {
        public SelectList GoalOptions { get; set; } = default!;
        public SelectList BillOptions { get; set; } = default!;

        public EditModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(data, logger, dropdownService) { }

        protected override async Task<Transaction> QueryRecordAsync(Guid id) {
            return await _data.TransactionRepository.GetDetailsAsync(id);
        }

        protected override async Task AfterQueryActionsAsync() {
            if (Record.SavingsGoalId.HasValue || Record.IsSavingsSpending || Record.Category.CategoryType == CategoryType.TRANSFER) {
                GoalOptions = _data.SavingsGoalRepository.GetForDropdown(Record.AccountId);
            }

            if (Record.Category.CategoryType == CategoryType.EXPENSE) {
                BillOptions = await _data.BillRepository.GetForDropdown(Record.AccountId);
            }
        }

        protected override async Task BeforeSaveActionsAsync() {
            Record.Date = Record.Date.AtMidnight()
                .Add(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));

            var category = await _data.CategoryRepository.GetByIdAsync(Record.CategoryId);
            var merchant = await _data.MerchantRepository.GetByIdAsync(Record.MerchantId);
            var account = await _data.AccountRepository.GetByIdAsync(Record.AccountId);

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
                    var savingsGoal = await _data.SavingsGoalRepository.GetByIdAsync(Record.SavingsGoalId);
                    Record.SavingsGoalName = savingsGoal.Name;
                }
            }

            if (Record.TransferId.HasValue) {
                var otherTransaction = _data.TransactionRepository
                    .GetMatchingFromTransferPair(Record.TransferId.Value, Record.Id);

                otherTransaction.Amount = -1 * Record.Amount;
                otherTransaction.Date = Record.Date;
                otherTransaction.Notes = Record.Notes;
                otherTransaction.TagId = Record.TagId;

                _data.TransactionRepository.Update(otherTransaction);
            }

            if (Record.IsSplit) {
                var childTransactions = (await _data.TransactionRepository
                    .GetSplitLines(Record.Id)).SplitTransactions;

                if (childTransactions.All(splt => splt.MerchantId == childTransactions[0].MerchantId)) {
                    childTransactions.ForEach(splt => {
                        splt.MerchantId = Record.MerchantId;
                        splt.MerchantName = Record.MerchantName;
                    });

                    _data.TransactionRepository.Update(childTransactions);
                }
            }
        }

        protected override IActionResult NavigationOnSuccess() {
            if (Record.IsSavingsSpending && !Record.SavingsGoalId.HasValue) {
                return RedirectToPage(PagePaths.SavingsGoalsAllocate, new { relatedId = Record.Id });
            }

            return RedirectToPage(PagePaths.TransactionIndex);
        }
    }
}
