using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Accounts {
    public class CreateModel : CreatePageModel<Account> {
        public CreateModel(AccountRepository repository) : base(repository) { }
    }
}
