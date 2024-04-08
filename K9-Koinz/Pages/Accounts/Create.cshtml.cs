using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using Newtonsoft.Json;

namespace K9_Koinz.Pages.Accounts {
    public class CreateModel : AbstractCreateModel<Account> {
        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger, dropdownService) { }

        protected override void BeforeSaveActions() {
            Record.InitialBalanceDate = Record.InitialBalanceDate.AtMidnight();
        }
    }
}
