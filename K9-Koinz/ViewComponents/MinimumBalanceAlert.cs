using K9_Koinz.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.ViewComponents {
    [ViewComponent(Name = "MinimumBalanceAlert")]
    public class MinimumBalanceAlert : ViewComponent {
        private readonly KoinzContext _context;

        public MinimumBalanceAlert(KoinzContext context) {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            var accounts = await _context.Accounts
                .Include(acct => acct.Transactions.Where(trans => trans.Date >= trans.Account.InitialBalanceDate))
                .AsNoTracking()
                .ToListAsync();

            accounts.ForEach(acct => acct.CurrentBalance = acct.Transactions.Sum(trans => trans.Amount) + acct.InitialBalance);
            return View(accounts);
        }
    }
}
