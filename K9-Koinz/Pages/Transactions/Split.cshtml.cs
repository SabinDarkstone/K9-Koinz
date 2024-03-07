using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace K9_Koinz.Pages.Transactions {
    public class SplitModel : AbstractDbPage {
        public SplitModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public Transaction ParentTransaction { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public List<Transaction> SplitTransactions { get; set; } = new List<Transaction>();

        public IActionResult OnGet(Guid parentId) {
            ParentTransaction = _context.Transactions
                .AsNoTracking()
                .Where(trans => trans.Id == parentId)
                .SingleOrDefault();

            SplitTransactions = _context.Transactions
                .AsNoTracking()
                .Where(splt => splt.ParentTransactionId == parentId)
                .OrderBy(splt => splt.CategoryName)
                .ToList();

            while (SplitTransactions.Count < 7) {
                SplitTransactions.Add(new Transaction {
                    AccountId = ParentTransaction.AccountId,
                    TagId = ParentTransaction.TagId,
                    MerchantId = ParentTransaction.MerchantId,
                    MerchantName = ParentTransaction.MerchantName,
                    ParentTransactionId = parentId
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var parent = _context.Transactions.Find(SplitTransactions[0].ParentTransactionId);

            foreach (var split in SplitTransactions) {
                if (split.CategoryId == null || split.CategoryId == Guid.Empty) {
                    continue;
                }

                var account = _context.Accounts.Find(parent.AccountId);
                var category = _context.Categories.Find(split.CategoryId);
                var merchant = _context.Merchants.Find(split.MerchantId);

                split.AccountName = account.Name;
                split.CategoryName = category.Name;
                split.MerchantName = merchant.Name;
                split.Date = parent.Date;

                _logger.LogInformation(JsonSerializer.Serialize(split, new JsonSerializerOptions { WriteIndented = true }));
            }

            var validSplits = SplitTransactions.Where(splt => splt.Amount != 0).ToList();
            if (validSplits.Any()) {
                parent.CategoryName = "Multiple";
                parent.IsSplit = true;
                parent.SplitTransactions = validSplits;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Transactions/Details", new { id = parent.Id });
        }
    }
}
