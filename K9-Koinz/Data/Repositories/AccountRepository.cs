using K9_Koinz.Models;
using K9_Koinz.Triggers;

namespace K9_Koinz.Data.Repositories {
    public class AccountRepository : TriggeredRepository<Account> {
        public AccountRepository(KoinzContext context, ITrigger<Account> trigger) : base(context, trigger) {
        }
    }
}
