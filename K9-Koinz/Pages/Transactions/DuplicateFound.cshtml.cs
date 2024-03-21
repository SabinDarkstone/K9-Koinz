using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transactions {
    public class DuplicateFoundModel : AbstractDbPage {

        private readonly IDupeCheckerService<Transaction> _dupeChecker;

        public Transaction Transaction { get; set; }
        public List<Transaction> MatchingTransactions { get; set; }

        public DuplicateFoundModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public async Task<IActionResult> OnGet(Guid? id) {
            if (!id.HasValue) {
                return NotFound();
            }

            Transaction = await _data.TransactionRepository.GetByIdAsync(id);
            MatchingTransactions = await _dupeChecker.FindPotentialDuplicates(Transaction);

            return Page();
        }

        public async Task<IActionResult> OnPost(Guid id, string mode) {
            if (mode == "cancel") {
                var transaction = await _data.TransactionRepository.GetByIdAsync(id);

                _data.TransactionRepository.Remove(transaction);

                if (transaction.TransferId.HasValue) {
                    var transfer = await _data.TransferRepository.GetByIdAsync(transaction.TransferId);
                    var otherTransaction = _data.TransactionRepository.GetMatchingFromTransferPair(transfer.Id, id);

                    if (otherTransaction != null) {
                        _data.TransactionRepository.Remove(otherTransaction);
                    }
                }

                _data.Save();
                return RedirectToPage(PagePaths.TransactionIndex);
            }

            var toTransaction = _data.TransactionRepository.GetWithCategory(id);
            if (toTransaction.Category.CategoryType == CategoryType.TRANSFER || toTransaction.Category.CategoryType == CategoryType.INCOME) {
                return RedirectToPage(PagePaths.SavingsGoalsAllocate, new { relatedId = id });
            }

            return RedirectToPage(PagePaths.TransactionIndex);
        }
    }
}
