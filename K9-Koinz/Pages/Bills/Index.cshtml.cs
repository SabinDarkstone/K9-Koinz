using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
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

    public class IndexModel : AbstractIndexModel<Bill> {
        public IndexModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public Dictionary<Guid, AccountSummary> AccountsWithBills { get; set; } = new();

        [Display(Name = "Show All Bills")]
        public bool ShowAllBills { get; set; }

        public async Task<IActionResult> OnGetAsync(bool? showAllBills) {
            var startDate = DateTime.Today.StartOfMonth();
            var endDate = DateTime.Today.EndOfMonth();

            if (showAllBills.HasValue) {
                this.ShowAllBills = showAllBills.Value;
            }

            if (showAllBills.HasValue && showAllBills.Value) {
                RecordList = await _data.BillRepository.GetAllBillsAsync();
            } else {
                RecordList = await _data.BillRepository.GetBillsWithinDateRangeAsync(startDate, endDate);
            }

            List<Account> accounts = RecordList.Select(bill => bill.Account).DistinctBy(acct => acct.Id).ToList();

            foreach (var account in accounts) {
                var billsForAccount = RecordList.Where(Bill => Bill.AccountId == account.Id).ToList();
                AccountsWithBills.Add(
                    account.Id,
                    new AccountSummary(account, billsForAccount)
                );
            }

            return Page();
        }

    }
}
