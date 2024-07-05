using System.Linq.Expressions;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace K9_Koinz.Services {
    
    public interface ITrendGraphService : ICustomService {
        public abstract Task<string> CreateGraphData(Expression<Func<Transaction, bool>> predicate, bool hideSavingsSpending, DateTime? startDate = null, DateTime? endDate = null);
    }

    public class TrendGraphService : AbstractService<TrendGraphService>, ITrendGraphService {
        public TrendGraphService(KoinzContext context, ILogger<TrendGraphService> logger) : base(context, logger) { }

        public async Task<string> CreateGraphData(Expression<Func<Transaction, bool>> predicate, bool hideSavingsSpending, DateTime? startDate = null, DateTime? endDate = null) {
            if (startDate == null) {
                startDate = DateTime.Today.AddMonths(-11).StartOfMonth();
            }
            if (endDate == null) {
                endDate = DateTime.Today.EndOfMonth();
            }

            var transactionsIQ = _context.Transactions
                .Include(trans => trans.Category)
                .Include(trans => trans.Merchant)
                .Include(trans => trans.Account)
                .Include(trans => trans.Tag)
                .AsNoTracking()
                .AsSplitQuery()
                .Where(trans => !trans.IsSplit)
                .Where(trans => !trans.Account.HideAccountTransactions)
                .Where(trans => trans.Date.Date >= startDate.Value.Date && trans.Date.Date <= endDate.Value.Date)
                .Where(predicate);

            if (hideSavingsSpending) {
                transactionsIQ = transactionsIQ.Where(trans => !trans.IsSavingsSpending);
            }

            var groups = (await transactionsIQ.ToListAsync())
                .GroupBy(trans => trans.Date.Month + "|" + trans.Date.Year)
                .ToDictionary(grp => grp.Key, grp => grp.ToList().GetTotal());

            foreach (var key in groups.Keys) {
                _logger.LogInformation(key.ToString());
                groups[key] *= -1;
            }

            var output = new List<SeriesColumn>();

            var startingKey = DateTime.Today.AddMonths(-11).Month + "|" + DateTime.Today.AddMonths(-11).Year;
            for (var i = 1; i < 12; i++) {
                var currentDate = DateTime.Today.AddMonths(-11 + i);
                var currentYear = currentDate.Year;
                var currentMonth = currentDate.Month;

                var amount = 0d;
                var month = DateUtils.GetMonthName(currentMonth).Substring(0, 3);

                if (groups.ContainsKey(currentMonth + "|" + currentYear)) {
                    amount = groups[currentMonth + "|" + currentYear];
                }

                output.Add(new SeriesColumn(
                    month + " '" + currentYear.ToString().Substring(2),
                    amount
                ));
            }

            return JsonConvert.SerializeObject(output);
        }
    }
}