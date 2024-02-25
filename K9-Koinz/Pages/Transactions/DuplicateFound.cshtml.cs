using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace K9_Koinz.Pages.Transactions {
    public class DuplicateFoundModel : PageModel {

        private KoinzContext _context;

        [BindProperty]
        public Transaction Transaction { get; set; }
        public List<Transaction> MatchingTransactions { get; set; }

        public DuplicateFoundModel(KoinzContext context) {
            _context = context;
        }

        public IActionResult OnGet(Guid id) {
            Transaction = _context.Transactions.Find(id);

            MatchingTransactions = _context.Transactions
                .Where(trans => trans.Amount == Transaction.Amount)
                .Where(trans => trans.MerchantId == Transaction.MerchantId)
                .AsEnumerable()
                .Where(trans => Math.Abs((trans.Date - Transaction.Date).TotalDays) <= 5)
                .Where(trans => trans.Id != id)
                .ToList();

            return Page();
        }

        public IActionResult OnPost(string mode) {
            if (mode == "cancel") {
                _context.Transactions.Remove(Transaction);
                _context.SaveChanges();
            }

            return RedirectToPage("Index");
        }
    }
}
