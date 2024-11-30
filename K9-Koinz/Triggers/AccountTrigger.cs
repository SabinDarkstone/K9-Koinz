using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers {
    public class AccountTrigger : GenericTrigger<Account>, ITrigger<Account> {
        public AccountTrigger(KoinzContext context) : base(context) { }
    }
}
