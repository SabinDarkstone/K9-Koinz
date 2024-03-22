using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Accounts {
    public class EditModel : AbstractEditModel<Account> {
        public EditModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(data, logger, dropdownService) { }

        protected override void BeforeSaveActions() {
            Record.InitialBalanceDate = Record.InitialBalanceDate.AtMidnight();
        }

        protected override async Task AfterSaveActionsAsync() {
            var relatedTransactions = await _data.Transactions.GetByAccountId(Record.Id);
            var relatedBills = await _data.Bills.GetByAccountId(Record.Id);

            foreach (var trans in relatedTransactions) {
                trans.AccountName = Record.Name;
            }

            foreach (var bill in relatedBills) {
                bill.AccountName = Record.Name;
            }

            _data.Transactions.Update(relatedTransactions);
            _data.Bills.Update(relatedBills);
            await _data.SaveAsync();
        }
    }
}
