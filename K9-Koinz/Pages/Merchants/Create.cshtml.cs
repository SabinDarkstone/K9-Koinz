using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Merchants {
    public class CreateModel : AbstractCreateModel<Merchant> {
        public CreateModel(KoinzContext context, IAccountService accountService, IAutocompleteService autocompleteService, ITagService tagService)
            : base(context, accountService, autocompleteService, tagService) { }

        protected override async Task AfterSaveActions() {
            return;
        }

        protected override async Task BeforeSaveActions() {
            return;
        }
    }
}
