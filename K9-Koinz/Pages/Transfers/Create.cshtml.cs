using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using K9_Koinz.Pages.Meta;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace K9_Koinz.Pages.Transfers {
    [Authorize]
    public class CreateModel : AbstractCreateModel<Transfer> {
        private Transaction[] transactions = new Transaction[2];
        private Transaction duplicateTransaction;

        private readonly IDupeCheckerService<Transaction> _dupeCheckerService;

        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger, IDropdownPopulatorService dropdownService,
            IDupeCheckerService<Transaction> dupeCheckerService)
            : base(context, logger, dropdownService) {
                _dupeCheckerService = dupeCheckerService;
            }

        protected override Task BeforePageLoadActions() {
            var defaultCategory = _context.Categories
                .Where(cat => cat.Name == "Transfer")
                .FirstOrDefault();
            Record = new Transfer {
                Category = defaultCategory,
                CategoryId = defaultCategory.Id,
            };

            return base.BeforePageLoadActions();
        }

        protected override void BeforeSaveActions() {
            Record.Date = Record.Date.AtMidnight() + DateTime.Now.TimeOfDay;

            if (Record.TagId == Guid.Empty) {
                Record.TagId = null;
            }
        }

        protected override async Task AfterSaveActionsAsync() {
            transactions = (await _context.CreateTransactionsFromTransfer(Record, false)).ToArray();
            
            foreach (var transaction in transactions) {
                if (transaction != null) {
                    var matchingTransactions = await _dupeCheckerService.FindPotentialDuplicates(transaction);
                    if (matchingTransactions.Count > 0) {
                        duplicateTransaction = transaction;
                        break;
                    }
                }
            }

            _context.Transactions.AddRange(transactions.Where(x => x != null));
            await _context.SaveChangesAsync();
        }

        protected override IActionResult NavigateOnSuccess() {
            if (duplicateTransaction != null) {
                return RedirectToPage(PagePaths.TransactionDuplicateFound, new { id = duplicateTransaction.Id });
            }

            var toAccount = _context.Accounts.Find(transactions[1].AccountId);
            var accountHasGoals = _context.SavingsGoals.Any(goal => goal.AccountId == toAccount.Id);

            if ((toAccount.Type == AccountType.CHECKING || toAccount.Type == AccountType.SAVINGS) && accountHasGoals) {
                return RedirectToPage(PagePaths.SavingsAllocate, new { relatedId = transactions[1].Id });
            }

            return RedirectToPage(PagePaths.TransactionIndex);
        }
    }
}
