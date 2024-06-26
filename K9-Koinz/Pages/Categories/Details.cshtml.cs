using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using System.Runtime.Serialization;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using Newtonsoft.Json;

namespace K9_Koinz.Pages.Categories {

    [DataContract]
    public class DataPoint {
        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "y")]
        public double? Y { get; set; }
    }

    public class DetailsModel : AbstractDetailsModel<Category> {

        public bool ChartError { get; set; }
        public string SpendingHistory { get; set; }

        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<Category> QueryRecordAsync(Guid id) {
            return await _context.Categories
                .Include(cat => cat.ParentCategory)
                .Include(cat => cat.ChildCategories)
                .AsNoTracking()
                .FirstOrDefaultAsync(cat => cat.Id == id);
        }

        protected override async Task AdditionalActionsAsync() {
            var spending = await GetSpendingHistory();
            if (spending != null) {
                SpendingHistory = JsonConvert.SerializeObject(spending);
                ChartError = false;
            } else {
                ChartError = true;
            }
        }

        private async Task<List<DataPoint>> GetSpendingHistory() {
            var transactions = await _context.Transactions
                .Include(trans => trans.Category)
                .Include(trans => trans.Account)
                .AsNoTracking()
                .Where(trans => trans.CategoryId == Record.Id || trans.Category.ParentCategoryId == Record.Id)
                .Where(trans => !trans.IsSavingsSpending)
                .Where(trans => !trans.IsSplit)
                .Where(trans => !trans.Account.HideAccountTransactions)
                .Where(trans => trans.Date <= DateTime.Today.Date.Date && trans.Date.Date >= DateTime.Today.AddMonths(-11))
                .ToListAsync();

            transactions.ForEach(trans => _logger.LogInformation(trans.Id + " " + trans.Amount.ToString()));

            var groups = transactions
                .GroupBy(trans => trans.Date.Month + "|" + trans.Date.Year)
                .ToDictionary(grp => grp.Key, grp => grp.ToList().GetTotal());

            foreach (var key in groups.Keys) {
                _logger.LogInformation(key.ToString());
                groups[key] *= -1;
            }

            var output = new List<DataPoint>();

            var startingKey = DateTime.Today.AddMonths(-11).Month + "|" + DateTime.Today.AddMonths(-11).Year;
            for (var i = 1; i < 12; i++) {
                var currentDate = DateTime.Today.AddMonths(-11 + i);
                var currentYear = currentDate.Year;
                var currentMonth = currentDate.Month;

                var amount = 0d;
                var month = DateUtils.GetMonthName(currentMonth);

                if (groups.ContainsKey(currentMonth + "|" + currentYear)) {
                    amount = groups[currentMonth + "|" + currentYear];
                }

                output.Add(new DataPoint {
                    Label = month + " '" + currentYear.ToString().Substring(2),
                    Y = amount
                });
            }

            return output;
        }
    }
}
