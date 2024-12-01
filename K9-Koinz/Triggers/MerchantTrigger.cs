using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers.Handlers.Merchants;

namespace K9_Koinz.Triggers {
    public class MerchantTrigger : GenericTrigger<Merchant> {
        public MerchantTrigger(KoinzContext context) : base(context) { }

        public override TriggerStatus OnAfterUpdate(List<Merchant> oldList, List<Merchant> newList) {
            new SetMerchantNameFields(context).Execute(oldList, newList);

            return TriggerStatus.SUCCESS;
        }
    }
}
