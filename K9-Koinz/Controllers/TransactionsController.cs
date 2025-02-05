using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Controllers {
    public class TransactionsController : Controller {

        private readonly KoinzContext _context;

        public TransactionsController(KoinzContext context) {
            _context = context;
        }

        private struct CreateTransactionResponse {
            public bool isSuccess { get; set; }
            public string errorMessage { get; set; }
            public Guid transactionId { get; set; }
        }

        public class CreateTransactionDTO {
            public string accountName { get; set; }
            public string merchantName { get; set; }
            public string categoryName { get; set; }
            public double amount { get; set; }
        }
        

        [HttpPost]
        public async Task<JsonResult> CreateTransaction([FromBody] CreateTransactionDTO dto) {
            var account = await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(acct => acct.Name == dto.accountName);
            var merchant = await _context.Merchants
                .AsNoTracking()
                .FirstOrDefaultAsync(merch => merch.Name == dto.merchantName);
                var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync (cat => cat.Name == dto.categoryName);

            if (account == null) {
                var errResponse = new CreateTransactionResponse {
                    isSuccess = false,
                    errorMessage = "Unknown account: " + dto.accountName,
                    transactionId = Guid.Empty
                };
                return Json(errResponse);
            } else if (category == null) {
                var errResponse = new CreateTransactionResponse {
                    isSuccess = false,
                    errorMessage = "Unknown category: " + dto.categoryName,
                    transactionId = Guid.Empty
                };
                return Json(errResponse);
            } else if (merchant == null) {
                var errResponse = new CreateTransactionResponse {
                    isSuccess = false,
                    errorMessage = "Unknown merchant: " + dto.merchantName,
                    transactionId = Guid.Empty
                };
                return Json(errResponse);
            }

            var newTransaction = new Transaction {
                AccountId = account.Id,
                AccountName = dto.accountName,
                MerchantId = merchant.Id,
                MerchantName = dto.merchantName,
                CategoryId = category.Id,
                CategoryName = dto.categoryName,
                Amount = dto.amount,
                Date = DateTime.Now,
                TransferId = null,
                BillId = null,
                SavingsGoalId = null,
                IsSavingsSpending = false,
                TagId = null,
                ParentTransactionId = null,
                Notes = "Created from API"
            };

            try {
                _context.Transactions.Add(newTransaction);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                var errResponse = new CreateTransactionResponse {
                    errorMessage = ex.Message,
                    isSuccess = false,
                    transactionId = Guid.Empty
                };
                return Json(errResponse);
            }

            var response = new CreateTransactionResponse {
                errorMessage = null,
                isSuccess = true,
                transactionId = newTransaction.Id
            };
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
