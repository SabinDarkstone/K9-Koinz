using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace K9_Koinz.Pages.Transactions.Split {
    [Authorize]
    public class CreateModel : AbstractDbPage {
        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public Transaction ParentTransaction { get; set; }
        public string ErrorMessage { get; set; }

        public SelectList SavingsGoalsList;

        [BindProperty]
        public List<Transaction> SplitTransactions { get; set; } = new List<Transaction>();

        public IActionResult OnGet(Guid parentId) {
            ParentTransaction = _context.Transactions
                .AsNoTracking()
                .Include(trans => trans.Category)
                .Where(trans => trans.Id == parentId)
                .SingleOrDefault();

            SplitTransactions = _context.Transactions
                .AsNoTracking()
                .Where(splt => splt.ParentTransactionId == parentId)
                .OrderBy(splt => splt.CategoryName)
                .ToList();

            while (SplitTransactions.Count < 40) {
                SplitTransactions.Add(new Transaction {
                    AccountId = ParentTransaction.AccountId,
                    TagId = ParentTransaction.TagId,
                    MerchantId = ParentTransaction.MerchantId,
                    MerchantName = ParentTransaction.MerchantName,
                    ParentTransactionId = parentId
                });
            }

            var savingsGoals = _context.SavingsGoals
                .AsNoTracking()
                .Where(goal => goal.AccountId == ParentTransaction.AccountId)
                .ToList();

            SavingsGoalsList = new SelectList(savingsGoals, nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var parent = _context.Transactions
                .Include(trans => trans.Category)
                .AsNoTracking()
                .Where(trans => trans.Id == SplitTransactions[0].ParentTransactionId)
                .FirstOrDefault();

            foreach (var split in SplitTransactions) {
                var isTransfer = parent.Category.CategoryType == CategoryType.TRANSFER;
                if (!isTransfer && split.CategoryId == null || split.CategoryId == Guid.Empty) {
                    continue;
                }

                if (isTransfer && split.SavingsGoalId == null || split.SavingsGoalId == Guid.Empty) {
                    continue;
                }

                var account = _context.Accounts.Find(parent.AccountId);

                Category category = null;
                SavingsGoal goal = null;
                if (!isTransfer) {
                    category = _context.Categories.AsNoTracking().Where(cat => cat.Id == split.CategoryId).FirstOrDefault();
                } else {
                    category = parent.Category;
                    goal = _context.SavingsGoals.Find(split.SavingsGoalId);
                    split.SavingsGoalName = goal.Name;
                }


                split.AccountName = account.Name;
                split.CategoryName = category.Name;
                split.Date = parent.Date;

                split.MerchantId = parent.MerchantId;
                split.MerchantName = parent.MerchantName;

                // Include the transfer ID from the parent transaction in the children
                split.TransferId = parent.TransferId;
            }

            var validSplits = SplitTransactions.Where(splt => splt.Amount != 0).ToList();
            if (validSplits.Any()) {
                if (parent.Category.CategoryType != CategoryType.TRANSFER) {
                    parent.CategoryName = "Multiple";
                }

                parent.IsSplit = true;
                parent.SplitTransactions = validSplits;
            }

            if (parent.TransferId.HasValue) {
                var transfer = _context.Transfers.Find(parent.TransferId);
                transfer.IsSplit = true;
            }

            _logger.LogInformation(validSplits.ToJson(Newtonsoft.Json.Formatting.Indented));

            _context.Update(parent);

            await _context.SaveChangesAsync();

            return RedirectToPage(PagePaths.TransactionDetails, new { id = parent.Id });
        }
    }
}
