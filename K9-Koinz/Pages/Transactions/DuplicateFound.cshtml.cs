using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace K9_Koinz.Pages.Transactions {
    [Authorize]
    public class DuplicateFoundModel : PageModel {

        private readonly KoinzContext _context;
        private readonly IDupeCheckerService<Transaction> _dupeChecker;

        public Transaction Transaction { get; set; }
        public List<Transaction> MatchingTransactions { get; set; }

        public DuplicateFoundModel(KoinzContext context, IDupeCheckerService<Transaction> dupeChecker) {
            _context = context;
            _dupeChecker = dupeChecker;
        }

        public async Task<IActionResult> OnGet(Guid? id) {
            if (!id.HasValue) {
                return NotFound();
            }

            Transaction = _context.Transactions.Find(id.Value);
            MatchingTransactions = await _dupeChecker.FindPotentialDuplicates(Transaction);

            return Page();
        }

        public IActionResult OnPost(Guid id, string mode) {
            var transactionFilterCookie = Request.Cookies["backToTransactions"].FromJson<TransactionNavPayload>();

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
                return RedirectToPage(PagePaths.TransactionIndex, routeValues: new {
                    sortOrder = transactionFilterCookie.SortOrder,
                    catFilter = transactionFilterCookie.CatFilter,
                    pageIndex = transactionFilterCookie.PageIndex,
                    accountFilter = transactionFilterCookie.AccountFilter,
                    minDate = transactionFilterCookie.MinDate,
                    maxDate = transactionFilterCookie.MaxDate,
                    merchFilter = transactionFilterCookie.MerchFilter
                });
            }

            var toTransaction = _context.Transactions
                .Include(trans => trans.Category)
                .Where(trans => trans.Id == id)
                .SingleOrDefault();
            if (toTransaction.Category.CategoryType == CategoryType.TRANSFER || toTransaction.Category.CategoryType == CategoryType.INCOME) {
                return RedirectToPage(PagePaths.SavingsAllocate, new { relatedId = id });
            }

            return RedirectToPage(PagePaths.TransactionIndex, routeValues: new {
                sortOrder = transactionFilterCookie.SortOrder,
                catFilter = transactionFilterCookie.CatFilter,
                pageIndex = transactionFilterCookie.PageIndex,
                accountFilter = transactionFilterCookie.AccountFilter,
                minDate = transactionFilterCookie.MinDate,
                maxDate = transactionFilterCookie.MaxDate,
                merchFilter = transactionFilterCookie.MerchFilter
            });
        }
    }
}
