using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Pages.Accounts {
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;

        public IndexModel(KoinzContext context) {
            _context = context;
        }

        public Dictionary<string, List<Account>> AccountDict { get;set; } = default!;

        public async Task OnGetAsync() {
            AccountDict = (await _context.Accounts
                .AsNoTracking()
                .ToListAsync())
                .GroupBy(acct => acct.Type.GetAttribute<DisplayAttribute>().Name)
                .ToDictionary(grp => grp.Key, grp => grp.OrderBy(acct => acct.Name).ToList());

            var savingsAccounts = new List<Account>();
            var checkingAccounts = new List<Account>();

            if (AccountDict.ContainsKey(AccountType.SAVINGS.GetAttribute<DisplayAttribute>().Name)) {
                savingsAccounts = AccountDict[AccountType.SAVINGS.GetAttribute<DisplayAttribute>().Name];
            }
            if (AccountDict.ContainsKey(AccountType.CHECKING.GetAttribute<DisplayAttribute>().Name)) {
                checkingAccounts = AccountDict[AccountType.CHECKING.GetAttribute<DisplayAttribute>().Name];
            }

            var checkingAndSavings = savingsAccounts.Concat(checkingAccounts).ToList();

            AccountDict["Checking & Savings"] = checkingAndSavings;
            AccountDict.Remove(AccountType.SAVINGS.GetAttribute<DisplayAttribute>().Name);
            AccountDict.Remove(AccountType.CHECKING.GetAttribute<DisplayAttribute>().Name);

            foreach (var acct in AccountDict.SelectMany(x => x.Value)) {
                var newBalance = _context.Transactions
                    .Where(trans => (trans.Date.Date > acct.InitialBalanceDate.Date || (trans.Date.Date == acct.InitialBalanceDate.Date && trans.DoNotSkip)) && trans.AccountId == acct.Id)
                    .GetTotal();
                acct.CurrentBalance = acct.InitialBalance + newBalance;
            }
        }
    }
}
