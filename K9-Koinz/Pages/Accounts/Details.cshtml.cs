using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Accounts {
    public class DetailsModel : DetailsPageModel<Account> {
        public DetailsModel(AccountRepository repository) : base(repository) { }

        public List<Transaction> Transactions { get; set; }

        protected override void AfterQueryActions() {
            Transactions = (_repository as AccountRepository).GetRecentTransactionsForAccount(Record.Id, 100);
            Record.CurrentBalance = (_repository as AccountRepository).GetCurrentBalance(Record.Id);
        }
    }
}
