using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Accounts {
    public class DetailsModel : AbstractDetailsModel<Account> {
        public DetailsModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Account> QueryRecordAsync(Guid id) {
            return await _data.Accounts.GetAccountDetails(id);
        }

        protected override void AdditionalActions() {
            var newBalance = _data.Transactions.GetTransactionTotalSinceBalanceSet(Record);
            Record.CurrentBalance = Record.InitialBalance + newBalance;
        }
    }
}
