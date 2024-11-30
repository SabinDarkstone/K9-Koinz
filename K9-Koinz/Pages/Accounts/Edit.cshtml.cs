using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Accounts {
    public class EditModel : EditPageModel<Account> {
        public EditModel(AccountRepository repository) : base(repository) { }
    }
}
