using Humanizer;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.SavingsGoals {
    public class CreateModel : AbstractCreateModel<SavingsGoal> {
        public CreateModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(data, logger, dropdownService) { }

        protected override void BeforeSaveActions() {
            var account = _context.Accounts.Find(Record.AccountId);
            Record.AccountName = account.Name;

            Record.SavedAmount = 0d;
        }
    }
}
