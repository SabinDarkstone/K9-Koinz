using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers.Handlers;
using K9_Koinz.Triggers.Handlers.Accounts;

namespace K9_Koinz.Triggers {
    public class AccountTrigger : GenericTrigger<Account>, ITrigger<Account> {
        public AccountTrigger(KoinzContext context) : base(context) {
            handlers = new Dictionary<string, AbstractTriggerHandler<Account>> {
                { "AccountNameFields", new AccountNameFields(context) }
            };
        }

        public override TriggerStatus OnAfterUpdate(List<Account> oldList, List<Account> newList) {
            (handlers["AccountNameFields"] as AccountNameFields).UpdateAccountNames(newList);

            return TriggerStatus.SUCCESS;
        }
    }
}
