using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Bills {
    public class EditModel : AbstractEditModel<Bill> {
        public EditModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
            : base(data, logger, dropdownService) { }

        protected override async Task<Bill> QueryRecordAsync(Guid id) {
            return await _data.Bills.GetDetails(id);
        }

        protected override async Task BeforeSaveActionsAsync() {
            var account = await _data.Accounts.GetByIdAsync(Record.AccountId);
            var merchant = await _data.Merchants.GetByIdAsync(Record.MerchantId);
            var category = await _data.Categories.GetByIdAsync(Record.CategoryId.Value);

            Record.AccountName = account.Name;
            Record.MerchantName = merchant.Name;
            Record.CategoryName = category.Name;

            if (Record.RepeatConfig.Mode == RepeatMode.SPECIFIC_DAY) {
                Record.RepeatConfig.IntervalGap = null;
            }
        }
    }
}
