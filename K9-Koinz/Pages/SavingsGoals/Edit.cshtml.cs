using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.SavingsGoals {
    public class EditModel : AbstractEditModel<SavingsGoal> {
        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger, dropdownService) { }

        protected override async Task BeforeSaveActionsAsync() {
            var account = await _context.Accounts.FindAsync(Record.AccountId);

            Record.AccountName = account.Name;
        }
    }
}
