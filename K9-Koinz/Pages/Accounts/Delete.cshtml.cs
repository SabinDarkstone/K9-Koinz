using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Accounts {
    public class DeleteModel : AbstractDeleteModel<Account> {
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }
    }
}
