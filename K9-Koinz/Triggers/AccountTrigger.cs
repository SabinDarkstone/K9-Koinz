using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers.Handlers.Accounts;

namespace K9_Koinz.Triggers {
    public class AccountTrigger : GenericTrigger<Account> {
        public AccountTrigger(KoinzContext context) : base(context) { }

        public override TriggerStatus OnAfterUpdate(List<Account> oldList, List<Account> newList) {
            new UpdateAccountNameFields(context).Execute(oldList, newList);
            return TriggerStatus.SUCCESS;
        }
    }
}
