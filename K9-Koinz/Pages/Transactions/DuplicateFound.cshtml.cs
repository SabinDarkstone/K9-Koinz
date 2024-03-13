using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transactions {
    public class DuplicateFoundModel : PageModel {

        private KoinzContext _context;

        public Transaction Transaction { get; set; }
        public List<Transaction> MatchingTransactions { get; set; }

        public DuplicateFoundModel(KoinzContext context) {
            _context = context;
        }

        public IActionResult OnGet(Guid? id) {
            if (!id.HasValue) {
                return NotFound();
            }

            Transaction = _context.Transactions.Find(id.Value);

            MatchingTransactions = _context.Transactions
                .Where(trans => trans.AccountId == Transaction.AccountId)
                .Where(trans => trans.Amount == Transaction.Amount || trans.Amount == -1 * Transaction.Amount)
                .AsEnumerable()
                .Where(trans => Math.Abs((trans.Date - Transaction.Date).TotalDays) <= 5)
                .Where(trans => trans.Id != id.Value)
                .ToList();

            return Page();
        }

        public IActionResult OnPost(Guid id, string mode) {
            if (mode == "cancel") {
                var transaction = _context.Transactions.Find(id);

                _context.Transactions.Remove(transaction);

                if (transaction.TransferId.HasValue) {
                    var transfer = _context.Transfers
                        .Where(fer => fer.Id == transaction.TransferId.Value)
                        .FirstOrDefault();

                    var otherTransaction = _context.Transactions
                        .Where(trans => trans.TransferId == transfer.Id)
                        .Where(trans => trans.Id != id)
                        .FirstOrDefault();

                    if (otherTransaction != null) {
                        _context.Transactions.Remove(otherTransaction);
                    }
                }

                _context.SaveChanges();
                return RedirectToPage(PagePaths.TransactionIndex);
            }

            var toTransaction = _context.Transactions
                .Include(trans => trans.Category)
                .Where(trans => trans.Id == id)
                .SingleOrDefault();
            if (toTransaction.Category.CategoryType == CategoryType.TRANSFER || toTransaction.Category.CategoryType == CategoryType.INCOME) {
                return RedirectToPage(PagePaths.SavingsGoalsAllocate, new { relatedId = id });
            }

            return RedirectToPage(PagePaths.TransactionIndex);
        }
    }
}
