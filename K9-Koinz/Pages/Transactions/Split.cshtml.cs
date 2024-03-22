using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.Transactions {
    public class SplitModel : AbstractDbPage {
        public SplitModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public Transaction ParentTransaction { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public List<Transaction> SplitTransactions { get; set; } = new List<Transaction>();

        public async Task<IActionResult> OnGetAsync(Guid parentId) {
            ParentTransaction = await _data.TransactionRepository.GetSplitLines(parentId);
            SplitTransactions = ParentTransaction.SplitTransactions;

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
            var parent = await _data.TransactionRepository
                .GetByIdAsync(SplitTransactions.First().ParentTransactionId);

            foreach (var split in SplitTransactions) {
                if (split.CategoryId == null || split.CategoryId == Guid.Empty) {
                    continue;
                }

                var category = await _data.CategoryRepository.GetByIdAsync(split.CategoryId);

                split.AccountName = parent.AccountName;
                split.CategoryName = category.Name;
                split.Date = parent.Date;

                split.MerchantId = parent.MerchantId;
                split.MerchantName = parent.MerchantName;
            }

            var validSplits = SplitTransactions.Where(splt => splt.Amount != 0).ToList();
            if (validSplits.Any()) {
                parent.CategoryName = "Multiple";
                parent.IsSplit = true;
                parent.SplitTransactions = validSplits;
            }

            await _data.SaveAsync();

            return RedirectToPage(PagePaths.TransactionDetails, new { id = parent.Id });
        }
    }
}
