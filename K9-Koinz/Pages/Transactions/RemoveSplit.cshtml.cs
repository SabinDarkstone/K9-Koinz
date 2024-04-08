using K9_Koinz.Data;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transactions {
    public class RemoveSplitModel : AbstractDbPage {
        public RemoveSplitModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public IActionResult OnGet(Guid parentId) {
            var parent = _context.Transactions
                .Include(trans => trans.SplitTransactions)
                .Include(trans => trans.Category)
                .Where(trans => trans.Id == parentId)
                .FirstOrDefault();

            parent.CategoryName = parent.Category.Name;
            parent.IsSplit = false;

            if (parent.TransferId.HasValue) {
                var transfer = _context.Transfers.Find(parent.TransferId);
                transfer.IsSplit = false;
            }

            _context.Transactions.RemoveRange(parent.SplitTransactions);

            parent.SplitTransactions = null;
            _context.Transactions.Update(parent);
            _context.SaveChanges();

            return RedirectToPage(PagePaths.TransactionDetails, new { id = parentId });
        }
    }
}
