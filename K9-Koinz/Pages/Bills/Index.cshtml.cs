using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Pages.Bills {
    public class IndexModel : IndexPageModel<Bill> {
        public IndexModel(BillRepository repository) : base(repository) { }
        public Dictionary<Guid, AccountSummary> AccountsWithBills { get; set; } = new();


        [Display(Name = "Show All Bills")]
        public bool ShowAllBills { get; set; }

        public async Task<IActionResult> OnGetAsync(string viewAll) {
            if (viewAll == "yes") {
                ShowAllBills = true;
            } else {
                ShowAllBills = false;
            }

            var startDate = DateTime.Today.StartOfMonth();
            var endDate = DateTime.Today.EndOfMonth();

            Records = await (_repository as BillRepository).GetBillList(ShowAllBills);
            List<Account> accounts = Records.Select(bill => bill.Account).DistinctBy(acct => acct.Id).ToList();

            foreach (var account in accounts) {
                var billsForAccount = Records.Where(Bill => Bill.AccountId == account.Id).ToList();
                AccountsWithBills.Add(
                    account.Id,
                    new AccountSummary(account, billsForAccount)
                );
            }

            return Page();
        }

    }
}
