using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Bills {
    public class CreateModel : AbstractCreateModel<Bill> {
        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IAccountService accountService, ITagService tagService)
                : base(context, logger, accountService, tagService) { }

        protected override async Task BeforeSaveActionsAsync() {
            var account = await _context.Accounts.FindAsync(Record.AccountId);
            var merchant = await _context.Merchants.FindAsync(Record.MerchantId);
            var category = await _context.Categories.FindAsync(Record.CategoryId);

            Record.AccountName = account.Name;
            Record.MerchantName = merchant.Name;
            Record.CategoryName = category.Name;

            if (Record.RepeatConfig.Mode == RepeatMode.SPECIFIC_DAY) {
                Record.RepeatConfig.IntervalGap = null;
            }
        }
    }
}
