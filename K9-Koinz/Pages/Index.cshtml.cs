using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis.Elfie.Extensions;

namespace K9_Koinz.Pages {

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
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;

        public IndexModel(KoinzContext context) {
            _context = context;
        }

        public IList<Budget> Budget { get; set; } = default!;

        public string ThisMonthSpendingJson { get; set; }
        public string LastMonthSpendingJson { get; set; }

        public async Task OnGetAsync() {
            var startOfThisMonth = DateTime.Now.StartOfMonth();
            var endOfThisMonth = DateTime.Now.EndOfMonth();

            var startOfLastMonth = DateTime.Now.AddMonths(-1).StartOfMonth();
            var endOfLastMonth = DateTime.Now.AddMonths(-1).EndOfMonth();

            var blockedCategoryIds = _context.Categories.Where(cat => cat.Name == "Transfer" || cat.Name == "Income" || (cat.ParentCategoryId.HasValue && (cat.ParentCategory.Name == "Transfer" || cat.ParentCategory.Name == "Income"))).Select(cat => cat.Id).ToList();

            var thisMonthTransactions = await _context.Transactions
                .Include(trans => trans.Account)
                .Where(trans => trans.Date >= startOfThisMonth && trans.Date <= endOfThisMonth)
                .Where(trans => trans.Account.Type == AccountType.CREDIT_CARD || trans.Account.Type == AccountType.CHECKING || trans.Account.Type == AccountType.SAVINGS)
                .Where(trans => blockedCategoryIds.Contains(trans.Category.Id) == false)
                .GroupBy(trans => trans.Date)
                .Select(group => new Point(group.Key.Day, group.Sum(trans => -1 * trans.Amount)))
                .ToListAsync();

            var lastMonthTransactions = await _context.Transactions
                .Include(trans => trans.Account)
                .Where(trans => trans.Date >= startOfLastMonth && trans.Date <= endOfLastMonth)
                .Where(trans => trans.Account.Type == AccountType.CREDIT_CARD || trans.Account.Type == AccountType.CHECKING || trans.Account.Type == AccountType.SAVINGS)
                .Where(trans => blockedCategoryIds.Contains(trans.Category.Id) == false)
                .GroupBy(trans => trans.Date)
                .Select(group => new Point(group.Key.Day, group.Sum(trans => -1 * trans.Amount)))
                .ToListAsync();

            ThisMonthSpendingJson = JsonConvert.SerializeObject(thisMonthTransactions.Accumulate().ToList().FillInGaps(DateTime.Now, false), Formatting.None, new JsonSerializerSettings {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            });

            LastMonthSpendingJson = JsonConvert.SerializeObject(lastMonthTransactions.Accumulate().ToList().FillInGaps(DateTime.Now.AddMonths(-1), true), Formatting.None, new JsonSerializerSettings {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            });
        }
    }
}
