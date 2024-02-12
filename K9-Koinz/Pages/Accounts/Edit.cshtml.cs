using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Accounts {
    public class EditModel : AbstractEditModel<Account> {
        public EditModel(KoinzContext context, IAccountService accountService,
            IAutocompleteService autocompleteService, ITagService tagService)
                : base(context, accountService, autocompleteService, tagService) { }

        protected override void BeforeSaveActions() {
            Record.InitialBalanceDate = Record.InitialBalanceDate.AtMidnight();
        }
    }
}
