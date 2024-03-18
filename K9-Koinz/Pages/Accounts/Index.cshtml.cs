using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.Accounts {
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;

        public IndexModel(KoinzContext context) {
            _context = context;
        }

        public IList<Account> Accounts { get;set; } = default!;

        public async Task OnGetAsync() {
            Accounts = await _context.Accounts
                .OrderBy(acct => acct.Name)
                .ToListAsync();

            foreach (Account acct in Accounts) {
                var newBalance = _context.Transactions
                    .Where(trans => (trans.Date.Date > acct.InitialBalanceDate.Date || (trans.Date.Date == acct.InitialBalanceDate.Date && trans.DoNotSkip)) && trans.AccountId == acct.Id)
                    .GetTotal();
                acct.CurrentBalance = acct.InitialBalance + newBalance;
            }
        }
    }
}
