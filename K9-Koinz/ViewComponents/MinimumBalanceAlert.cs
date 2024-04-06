using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
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
                .Include(acct => acct.Transactions.Where(trans => trans.Date.Date >= trans.Account.InitialBalanceDate))
            .AsNoTracking()
            .ToListAsync();

            foreach (var acct in accounts) {
                acct.Transactions = acct.Transactions.Where(trans => trans.Date.Date > acct.InitialBalanceDate || trans.Date.Date == acct.InitialBalanceDate && trans.DoNotSkip).ToList();
                acct.CurrentBalance = acct.Transactions.GetTotal() + acct.InitialBalance;
            }

            return View(accounts);
        }
    }
}
