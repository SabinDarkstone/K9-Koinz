using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Pages.Bills {
    public struct AccountSummary {
        public string Name { get; init; }
        public double AmountDueThisMonth { get; init; }

        public AccountSummary(Account account, List<Bill> bills) {
            Name = account.Name;
            AmountDueThisMonth = bills
                .Where(bill => bill.AccountId == account.Id)
                .GetTotal();
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

        [Display(Name = "Show All Bills")]
        public bool ShowAllBills { get; set; }

        public async Task<IActionResult> OnPostAsync(bool? showAllBills) {
            ShowAllBills = showAllBills ?? false;
            return await OnGetAsync();
        }

        public async Task<IActionResult> OnGetAsync() {
            var startDate = DateTime.Today.StartOfMonth();
            var endDate = DateTime.Today.EndOfMonth();

            if (ShowAllBills) {
                Bills = (await _context.Bills
                    .AsNoTracking()
                    .Include(bill => bill.Account)
                    .Include(bill => bill.RepeatConfig)
                    .ToListAsync())
                    .OrderBy(bill => bill.RepeatConfig.NextFiring)
                    .ToList();
            } else {
                Bills = (await _context.Bills
                    .AsNoTracking()
                    .Include(bill => bill.Account)
                    .Include(bill => bill.RepeatConfig)
                    .ToListAsync())
                    .Where(bill => bill.RepeatConfig.NextFiring.HasValue)
                    .Where(bill => bill.RepeatConfig.NextFiring >= startDate)
                    .Where(bill => bill.RepeatConfig.NextFiring <= endDate)
                    .OrderBy(bill => bill.RepeatConfig.NextFiring)
                    .ToList();
            }

            List<Account> accounts = Bills.Select(bill => bill.Account).DistinctBy(acct => acct.Id).ToList();

            foreach (var account in accounts) {
                var billsForAccount = Bills.Where(Bill => Bill.AccountId == account.Id).ToList();
                AccountsWithBills.Add(
                    account.Id,
                    new AccountSummary(account, billsForAccount)
                );
            }

            return Page();
        }

    }
}
