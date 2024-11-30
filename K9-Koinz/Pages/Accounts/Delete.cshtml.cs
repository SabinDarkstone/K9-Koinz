using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Accounts {
    public class DeleteModel : DeletePageModel<Account> {
        public DeleteModel(AccountRepository repository) : base(repository) { }
    }
}
