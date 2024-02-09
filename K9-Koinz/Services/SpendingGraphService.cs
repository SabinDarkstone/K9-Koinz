using K9_Koinz.Data;
using K9_Koinz.Models;
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

    public class SpendingGraphService : AbstractService<SpendingGraphService>, ICustomService {
        public SpendingGraphService(KoinzContext context, ILogger<SpendingGraphService> logger) : base(context, logger) { }

        public async Task<string[]> CreateGraphData() {
            var startOfThisMonth = DateTime.Now.StartOfMonth();
            var endOfThisMonth = DateTime.Now.EndOfMonth();

            var startOfLastMonth = DateTime.Now.AddMonths(-1).StartOfMonth();
            var endOfLastMonth = DateTime.Now.AddMonths(-1).EndOfMonth();

            var thisMonthTransactions = await _context.Transactions
                .Include(trans => trans.Account)
                .Where(trans => trans.Date >= startOfThisMonth && trans.Date <= endOfThisMonth)
                .Where(trans => trans.Account.Type == AccountType.CREDIT_CARD || trans.Account.Type == AccountType.CHECKING || trans.Account.Type == AccountType.SAVINGS)
                .Where(trans => trans.Category.CategoryType == CategoryType.EXPENSE)
                .GroupBy(trans => trans.Date)
                .Select(group => new Point(group.Key.Day, group.Sum(trans => -1 * trans.Amount)))
                .ToListAsync();

            var lastMonthTransactions = await _context.Transactions
                .Include(trans => trans.Account)
                .Where(trans => trans.Date >= startOfLastMonth && trans.Date <= endOfLastMonth)
                .Where(trans => trans.Account.Type == AccountType.CREDIT_CARD || trans.Account.Type == AccountType.CHECKING || trans.Account.Type == AccountType.SAVINGS)
                .Where(trans => trans.Category.CategoryType == CategoryType.EXPENSE)
                .GroupBy(trans => trans.Date)
                .Select(group => new Point(group.Key.Day, group.Sum(trans => -1 * trans.Amount)))
                .ToListAsync();

            var thisMonthSpendingJson = JsonConvert.SerializeObject(thisMonthTransactions.Accumulate().ToList().FillInGaps(DateTime.Now, false), Formatting.None, new JsonSerializerSettings {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            });

            var lastMonthSpendingJson = JsonConvert.SerializeObject(lastMonthTransactions.Accumulate().ToList().FillInGaps(DateTime.Now.AddMonths(-1), true), Formatting.None, new JsonSerializerSettings {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            });

            return [thisMonthSpendingJson, lastMonthSpendingJson];
        }
    }
}
