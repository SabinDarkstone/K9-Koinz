using K9_Koinz.Data;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Controllers {
    public class TransactionsController : GenericController {
        public TransactionsController(IRepositoryWrapper data)
            : base(data) { }

        public async Task<JsonResult> GetTransactionsFromTrendAsync(string categoryId, string month, int year) {
            var monthNumber = DateUtils.GetMonthNumber(month);
            var daysInSelectedMonth = DateTime.DaysInMonth(year, DateUtils.GetMonthNumber(month));
            var startDate = new DateTime(year, monthNumber, 1);
            var endDate = new DateTime(year, monthNumber, daysInSelectedMonth);

            var transactions = await _data.Transactions.GetForTrendGraph(
                Guid.Parse(categoryId), startDate, endDate
            );

            return new JsonResult(transactions);
        }
    }
}
