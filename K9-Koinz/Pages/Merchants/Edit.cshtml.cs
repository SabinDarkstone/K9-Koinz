using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Merchants {
    public class EditModel : AbstractEditModel<Merchant> {
        public EditModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(data, logger, dropdownService) { }

        protected override async Task AfterSaveActionsAsync() {
            var relatedTransactions = await _data.TransactionRepository.GetByMerchant(Record.Id);
            var relatedBills = await _data.BillRepository.GetByMerchant(Record.Id);

            foreach (var trans in relatedTransactions) {
                trans.MerchantName = Record.Name;
            }

            foreach (var bill in relatedBills) {
                bill.MerchantName = Record.Name;
            }

            _data.TransactionRepository.Update(relatedTransactions);
            _data.BillRepository.Update(relatedBills);
            await _data.SaveAsync();
        }
    }
}
