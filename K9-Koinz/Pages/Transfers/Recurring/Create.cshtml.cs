using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transfers.Recurring {
    public class CreateModel : AbstractCreateModel<Transfer> {
        private Transaction[] transactions = new Transaction[2];
        private bool foundMatchingSchedule;

        public Category DefaultCategory { get; set; }

        public CreateModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger, IDropdownPopulatorService dropdownService)
            : base(data, logger, dropdownService) { }

        protected override Task BeforePageLoadActions() {
            DefaultCategory = _data.CategoryRepository.GetByName("Transfer");
            Record = new Transfer();
            Record.CategoryId = DefaultCategory.Id;

            return base.BeforePageLoadActions();
        }

        protected override async Task BeforeSaveActionsAsync() {
            foundMatchingSchedule = (await _data.TransferRepository.FindDuplicates(Record)).Any();

            if (Record.TagId == Guid.Empty) {
                Record.TagId = null;
            }
        }

        protected override async Task AfterSaveActionsAsync() {
            if (Record.RepeatConfig.NeedsToFireImmediately) {
                transactions = (await _data.CreateTransactionsFromTransfer(Record)).ToArray();
                Record.RepeatConfig.FireNow();

                foreach (var transaction in transactions) {
                    transaction.TransferId = Record.Id;
                }

                _data.TransferRepository.Update(Record);
                _data.TransactionRepository.Add(transactions);

                await _data.SaveAsync();
            }
        }

        protected override IActionResult NavigateOnSuccess() {
            if (foundMatchingSchedule) {
                return RedirectToPage(PagePaths.TransferDuplicateFound, new { id = Record.Id });
            }

            var toAccount = _data.AccountRepository.GetByIdAsync(Record.ToAccountId).Result;
            var accountHasGoals = _data.SavingsGoalRepository.DoesExistAsync(Record.ToAccountId).Result;

            if ((toAccount.Type == AccountType.CHECKING || toAccount.Type == AccountType.SAVINGS) && accountHasGoals) {
                return RedirectToPage(PagePaths.SavingsGoalsAllocateRecurring, new { relatedId = Record.Id });
            }

            return RedirectToPage(PagePaths.TransferManage);
        }
    }
}
