using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace K9_Koinz.Services {

    [DataContract]
    public class Bar {
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "y")]
        public double Value { get; set; }
    }

    public interface ICashflowGraphService : ICustomService {
        public abstract Task<string[]> CreateGraphData(bool excludeHidden, bool excludeBills);
    }

    public class CashflowGraphService : AbstractService<CashflowGraphService>, ICashflowGraphService {
        public CashflowGraphService(KoinzContext context, ILogger<CashflowGraphService> logger) : base(context, logger) { }

        public async Task<string[]> CreateGraphData(bool excludeHidden, bool excludeBills) {
            var jsonSettings = new JsonSerializerSettings {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };

            var income = await GetDataForSeries("income", excludeHidden, excludeBills);
            var expenses = await GetDataForSeries("expenses", excludeHidden, excludeBills);

            var incomeJson = JsonConvert.SerializeObject(income, Formatting.None, jsonSettings);
            var expenseJson = JsonConvert.SerializeObject(expenses, Formatting.None, jsonSettings);

            return [incomeJson, expenseJson];
        }

        private async Task<List<Bar>> GetDataForSeries(string seriesType, bool excludeHidden, bool excludeBills) {
            var startDate = DateTime.Today.StartOfMonth().AddMonths(-11);
            var endDate = DateTime.Today.EndOfMonth();

            IQueryable<Transaction> query = _context.Transactions
                .AsNoTracking()
                .Include(trans => trans.Account)
                .Include(trans => trans.Category)
                .Where(trans => trans.Date.Date >= startDate && trans.Date.Date <= endDate)
                .Where(trans => !trans.IsSplit)
                .Where(trans => !trans.Account.HideAccountTransactions)
                .Where(trans => trans.Account.Type == AccountType.CREDIT_CARD || trans.Account.Type == AccountType.CHECKING || trans.Account.Type == AccountType.SAVINGS);

            if (seriesType == "expenses") {
                query = query.Where(trans => trans.Category.CategoryType == CategoryType.EXPENSE || trans.Category.CategoryType == CategoryType.TRANSFER);
            } else if (seriesType == "income") {
                query = query.Where(trans => trans.Category.CategoryType == CategoryType.INCOME);
            }

            if (excludeHidden) {
                query = query.Where(trans => !trans.IsSavingsSpending);
            }

            if (excludeBills) {
                query = query.Where(trans => trans.BillId == null);
            }

            var transList = await query.ToListAsync();
            var grouped = transList.GroupBy(trans => trans.Date.Month + "|" + trans.Date.Year)
                .ToDictionary(grp => grp.Key, grp => grp.ToList());

            var monthlyDict = new Dictionary<string, double>();
            foreach (var month in grouped.Keys) {
                var runningTotal = 0d;
                foreach (var trans in grouped[month]) {
                    if (trans.SavingsGoalId != null && trans.Amount > 0 && seriesType == "expenses" && trans.Category.CategoryType == CategoryType.TRANSFER) {
                        runningTotal += trans.Amount * -1;
                    } else {
                        runningTotal += trans.Amount * (seriesType == "expenses" ? -1 : 1);
                    }
                }
                monthlyDict[month] = runningTotal;
            }

            List<Bar> output = new();
            var startingKey = DateTime.Today.AddMonths(-11).Month + "|" + DateTime.Today.AddMonths(-11).Year;
            for (var i = 1; i < 12; i++) {
                var currentDate = DateTime.Today.AddMonths(-11 + i);
                var currentYear = currentDate.Year;
                var currentMonth = currentDate.Month;

                var amount = 0d;
                var month = DateUtils.GetMonthName(currentMonth);

                if (monthlyDict.ContainsKey(currentMonth + "|" + currentYear)) {
                    amount = monthlyDict[currentMonth + "|" + currentYear];
                }

                output.Add(new Bar {
                    Label = month + " '" + currentYear.ToString().Substring(2),
                    Value = amount
                });
            }

            return output;
        }
    }
}
