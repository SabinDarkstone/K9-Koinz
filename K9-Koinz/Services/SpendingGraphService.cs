﻿using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace K9_Koinz.Services {

    [DataContract]
    public class Point {
        [DataMember(Name = "x")]
        public double X { get; set; }
        [DataMember(Name = "y")]
        public double Y { get; set; }

        public Point(double x, double y) {
            X = x;
            Y = y;
        }
    }

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
                thisMonthSpendingJson = JsonConvert.SerializeObject(thisMonthTransactions.Accumulate().ToList().FillInGaps(DateTime.Now, false), Formatting.None, new JsonSerializerSettings {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                });
            }

            var lastMonthTransactions = await getTransactionsForGraph(startOfLastMonth, endOfLastMonth);
            if (lastMonthTransactions.Count > 0) {
                lastMonthSpendingJson = JsonConvert.SerializeObject(lastMonthTransactions.Accumulate().ToList().FillInGaps(DateTime.Now.AddMonths(-1), true), Formatting.None, new JsonSerializerSettings {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                });
            }

            if (_context.Transactions.Any(trans => trans.Date >= startOfThreeMonthsAgo && trans.Date <= endOfMonthThreeMonthsAgo)) {
                var lastThreeMonthTransactions = await getTransactionsForGraph(startOfThreeMonthsAgo, endOfLastMonth, true);
                threeMonthAverageSpendingJson = JsonConvert.SerializeObject(lastThreeMonthTransactions.Accumulate().ToList().FillInGaps(DateTime.Now.AddMonths(-1), true), Formatting.None, new JsonSerializerSettings {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                });
            }

            return [thisMonthSpendingJson, lastMonthSpendingJson, threeMonthAverageSpendingJson];
        }

        private async Task<List<Point>> getTransactionsForGraph(DateTime startDate, DateTime endDate, bool doAverage = false) {
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
                    .Select(group => new Point(group.Key, group.ToList().GetTotal(true) / 3))
                    .ToListAsync();
            } else {
                return await query
                    .Select(group => new Point(group.Key, group.ToList().GetTotal(true)))
                    .ToListAsync();
            }
        }
    }
}
