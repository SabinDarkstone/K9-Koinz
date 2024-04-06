using K9_Koinz.Data;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Controllers {
    public class TransactionsController : Controller {

        private readonly KoinzContext _context;

        public TransactionsController(KoinzContext context) {
            _context = context;
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
