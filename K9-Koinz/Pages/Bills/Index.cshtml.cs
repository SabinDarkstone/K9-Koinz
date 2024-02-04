using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Bills {
    public struct AccountSummary {
        public string Name { get; init; }
        public double AmountDueThisMonth { get; init; }

        public AccountSummary(Account account, List<Bill> bills) {
            Name = account.Name;
            AmountDueThisMonth = bills
                .Where(bill => bill.AccountId == account.Id)
                .Sum(bill => bill.BillAmount);
        }

        public AccountSummary(Account account) {
            Name = account.Name;
            AmountDueThisMonth = 0d;
        }
    }

    public class IndexModel : PageModel {
        private readonly KoinzContext _context;

        public IndexModel(KoinzContext context) {
            _context = context;
        }

        public List<Bill> Bills { get; set; } = default;
        public Dictionary<Guid, AccountSummary> AccountsWithBills { get; set; } = new();

        public async Task<IActionResult> OnGetAsync() {
            var startDate = DateTime.Today.StartOfMonth();
            var endDate = DateTime.Today.EndOfMonth();
            Bills = (await _context.Bills
                .Include(bill => bill.Account)
                .ToListAsync())
                .Where(bill => bill.NextDueDate >= startDate)
                .Where(bill => bill.NextDueDate <= endDate)
                .OrderBy(bill => bill.NextDueDate)
                .ToList();

            var accounts = await _context.Accounts.ToListAsync();
            accounts.ForEach(acct => AccountsWithBills.Add(acct.Id, new AccountSummary(acct, Bills)));

            foreach (var bill in Bills) {
                if (!AccountsWithBills.ContainsKey(bill.AccountId)) {
                    AccountsWithBills.Add(bill.AccountId, new AccountSummary(bill.Account));
                }
            }

            return Page();
        }

    }
}
