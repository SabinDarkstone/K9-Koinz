using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transactions {
    public class EditModel : AbstractEditModel<Transaction> {
        public SelectList GoalOptions { get; set; } = default!;

        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IAccountService accountService, IAutocompleteService autocompleteService,
            ITagService tagService)
                : base(context, logger, accountService, autocompleteService, tagService) { }

        protected override async Task<Transaction> QueryRecordAsync(Guid id) {
            return await _context.Transactions
                .Include(trans => trans.Category)
                .SingleOrDefaultAsync(trans => trans.Id == id);
        }

        protected override async Task AfterQueryActionsAsync() {
            if (Record.SavingsGoalId.HasValue || Record.IsSavingsSpending || Record.Category.CategoryType == CategoryType.TRANSFER) {
                GoalOptions = new SelectList(await _context.SavingsGoals
                    .Where(goal => goal.AccountId == Record.AccountId)
                    .ToListAsync(), nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
            }
        }

        protected override async Task BeforeSaveActionsAsync() {
            Record.Date = Record.Date.AtMidnight()
                .Add(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));

            var category = await _context.Categories.FindAsync(Record.CategoryId);
            var merchant = await _context.Merchants.FindAsync(Record.MerchantId);
            var account = await _context.Accounts.FindAsync(Record.AccountId);
            Record.CategoryName = category.Name;
            Record.MerchantName = merchant.Name;
            Record.AccountName = account.Name;

            if (Record.TagId == Guid.Empty) {
                Record.TagId = null;
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
        }

        protected override IActionResult NavigationOnSuccess() {
            if (Record.IsSavingsSpending && !Record.SavingsGoalId.HasValue) {
                return RedirectToPage("/SavingsGoals/Allocate", new { relatedId = Record.Id });
            }

            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnGetMerchantAutoComplete(string text) {
            return await _autocompleteService.AutocompleteMerchantsAsync(text.Trim());
        }

        public async Task<IActionResult> OnGetCategoryAutoComplete(string text) {
            return await _autocompleteService.AutocompleteCategoriesAsync(text.Trim());
        }
    }
}
