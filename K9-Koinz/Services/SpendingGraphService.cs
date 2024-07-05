using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace K9_Koinz.Services {

    public interface ISpendingGraphService : ICustomService {
        public abstract Task<string[]> CreateGraphData();
    }

    public class SpendingGraphService : AbstractService<SpendingGraphService>, ISpendingGraphService {
        public SpendingGraphService(KoinzContext context, ILogger<SpendingGraphService> logger) : base(context, logger) { }

        public async Task<string[]> CreateGraphData() {
            var startOfThisMonth = DateTime.Now.StartOfMonth();
            var endOfThisMonth = DateTime.Now.EndOfMonth();

            var startOfLastMonth = DateTime.Now.AddMonths(-1).StartOfMonth();
            var endOfLastMonth = DateTime.Now.AddMonths(-1).EndOfMonth();

            var startOfThreeMonthsAgo = DateTime.Now.AddMonths(-3).StartOfMonth();
            var endOfMonthThreeMonthsAgo = startOfThreeMonthsAgo.EndOfMonth();

            string lastMonthSpendingJson = "[]";
            string thisMonthSpendingJson = "[]";
            string threeMonthAverageSpendingJson = "[]";

            var thisMonthTransactions = await getTransactionsForGraph(startOfThisMonth, endOfThisMonth);
            if (thisMonthTransactions.Count > 0) {
                thisMonthSpendingJson = Serialize(GraphifyData(thisMonthTransactions, DateTime.Now, false));
            }

            var lastMonthTransactions = await getTransactionsForGraph(startOfLastMonth, endOfLastMonth);
            if (lastMonthTransactions.Count > 0) {
                lastMonthSpendingJson = Serialize(GraphifyData(lastMonthTransactions, DateTime.Now.AddMonths(-1), true));
            }

            if (_context.Transactions.Any(trans => trans.Date >= startOfThreeMonthsAgo && trans.Date <= endOfMonthThreeMonthsAgo)) {
                var lastThreeMonthTransactions = await getTransactionsForGraph(startOfThreeMonthsAgo, endOfLastMonth, true);
                threeMonthAverageSpendingJson = Serialize(GraphifyData(lastThreeMonthTransactions, DateTime.Now.AddMonths(-1), true));
            }

            return [thisMonthSpendingJson, lastMonthSpendingJson, threeMonthAverageSpendingJson];
        }

        private List<SeriesLine> GraphifyData(List<SeriesLine> seriesData, DateTime refDate, bool doFullMonth) {
            return seriesData.Accumulate().ToList().FillInGaps(refDate, doFullMonth);
        }

        private string Serialize(List<SeriesLine>seriesData) {
            return JsonConvert.SerializeObject(seriesData, Formatting.None, JsonUtils.DefaultSettings);
        }

        private async Task<List<SeriesLine>> getTransactionsForGraph(DateTime startDate, DateTime endDate, bool doAverage = false) {
            var query = _context.Transactions
                .Include(trans => trans.Account)
                .Where(trans => trans.Date >= startDate && trans.Date <= endDate)
                .Where(trans => trans.Account.Type == AccountType.CREDIT_CARD || trans.Account.Type == AccountType.CHECKING || trans.Account.Type == AccountType.SAVINGS)
                .Where(trans => trans.Category.CategoryType == CategoryType.EXPENSE)
                .Where(trans => !trans.IsSavingsSpending)
                .Where(trans => !trans.IsSplit)
                .Where(trans => !trans.Account.HideAccountTransactions)
                .GroupBy(trans => trans.Date.Day);

            if (doAverage) {
                return await query
                    .Select(group => new SeriesLine(group.Key, group.ToList().GetTotal(true) / 3))
                    .ToListAsync();
            } else {
                return await query
                    .Select(group => new SeriesLine(group.Key, group.ToList().GetTotal(true)))
                    .ToListAsync();
            }
        }
    }
}
