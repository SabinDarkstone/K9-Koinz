using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transactions {
    public class DuplicateFoundModel : PageModel {

        private KoinzContext _context;

        public Transaction Transaction { get; set; }
        public Transaction OtherTransaction { get; set; }

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
                .Where(trans => trans.MerchantId == Transaction.MerchantId)
                .AsEnumerable()
                .Where(trans => Math.Abs((trans.Date - Transaction.Date).TotalDays) <= 5)
                .Where(trans => trans.Id != id.Value)
                .ToList();

            return Page();
        }

        public IActionResult OnPost(Guid id, string mode) {
            if (mode == "cancel") {
                var transactions = _context.Transactions
                    .Where(trans => trans.Id == id)
                    .ToList();

                _context.Transactions.RemoveRange(transactions);
                _context.SaveChanges();
                return RedirectToPage("/Transactions/Index");
            }

            var toTransaction = _context.Transactions
                .Include(trans => trans.Category)
                .Where(trans => trans.Id == id)
                .SingleOrDefault();
            if (toTransaction.Category.CategoryType == CategoryType.TRANSFER || toTransaction.Category.CategoryType == CategoryType.INCOME) {
                return RedirectToPage("/SavingsGoals/Allocate", new { relatedId = id });
            }

            return RedirectToPage("/Transactions/Index");
        }
    }
}
