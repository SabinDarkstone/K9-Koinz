using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using System.ComponentModel.DataAnnotations;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Accounts {
    public class IndexModel : AbstractDbPage {
        public IndexModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public Dictionary<string, List<Account>> AccountDict { get;set; } = default!;

        public async Task OnGetAsync() {
            AccountDict = await _data.Accounts.GetAllGroupedByType();

            var savingsAccountGroup = AccountDict[AccountType.SAVINGS.GetAttribute<DisplayAttribute>().Name];
            var checkingAndSavings = AccountDict[AccountType.CHECKING.GetAttribute<DisplayAttribute>().Name].Concat(savingsAccountGroup).OrderBy(acct => acct.Name).ToList();
            AccountDict["Checking & Savings"] = checkingAndSavings;
            AccountDict.Remove(AccountType.SAVINGS.GetAttribute<DisplayAttribute>().Name);
            AccountDict.Remove(AccountType.CHECKING.GetAttribute<DisplayAttribute>().Name);

            foreach (var acct in AccountDict.SelectMany(x => x.Value)) {
                var newBalance = _data.Transactions.GetTransactionTotalSinceBalanceSet(acct);
                acct.CurrentBalance = acct.InitialBalance + newBalance;
            }
        }
    }
}
