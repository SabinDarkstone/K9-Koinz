using K9_Koinz.Data;
using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Triggers;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Controllers {
    public class TransactionsController : Controller {

        private readonly KoinzContext _context;
        private readonly TransactionRepository _repo;

        public TransactionsController(KoinzContext context, TransactionRepository repo) {
            _context = context;
            _repo = repo;
        }

        private struct CreateTransactionResponse {
            public bool isSuccess { get; set; }
            public string errorMessage { get; set; }
            public Guid transactionId { get; set; }
         }
        

        [HttpPost]
        public async Task<JsonResult> CreateTransaction([FromBody] Transaction inputTransaction) {
            CreateTransactionResponse response;

            try {
                inputTransaction.IsSavingsSpending = false;
                inputTransaction.IsSplit = false;
                inputTransaction.BillId = null;
                inputTransaction.TagId = null;
                inputTransaction.TransferId = null;
                inputTransaction.Date = DateTime.Now;

                var result = await _repo.AddAsync(inputTransaction);

                if (result.IsSuccess) {
                    response = new CreateTransactionResponse {
                        errorMessage = null,
                        isSuccess = true,
                        transactionId = result.Ids.FirstOrDefault()
                    };
                } else {
                    throw new Exception(result.ErrorMessage);
                }
            } catch (Exception ex) {
                response = new CreateTransactionResponse {
                    errorMessage = ex.Message,
                    isSuccess = false,
                    transactionId = Guid.Empty
                };
            }

            return Json(response);
        }

        public async Task<JsonResult> GetTransactionsFromTrendAsync(string categoryId, string month, int year) {
            var monthNumber = DateUtils.GetMonthNumber(month);
            var daysInSelectedMonth = DateTime.DaysInMonth(year, DateUtils.GetMonthNumber(month));
            var startDate = new DateTime(year, monthNumber, 1);
            var endDate = new DateTime(year, monthNumber, daysInSelectedMonth);

            var transactions = await _context.Transactions
                .Where(trans => trans.CategoryId == Guid.Parse(categoryId) || (trans.Category.ParentCategoryId.HasValue && trans.Category.ParentCategoryId.Value == Guid.Parse(categoryId)))
                .Where(trans => trans.Date.Date >= startDate && trans.Date.Date <= endDate)
                .Where(trans => !trans.IsSavingsSpending)
                .Where(trans => !trans.IsSplit)
                .OrderBy(trans => trans.Amount)
                .ToListAsync();

            return new JsonResult(transactions);
        }
    }
}
