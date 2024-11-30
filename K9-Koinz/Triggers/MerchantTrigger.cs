using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers.Handlers.Merchants;

namespace K9_Koinz.Triggers {
    public class MerchantTrigger : GenericTrigger<Merchant>, ITrigger<Merchant> {
        public MerchantTrigger(KoinzContext context) : base(context) {
            handlers = new Dictionary<string, Handlers.AbstractTriggerHandler<Merchant>> {
                { "nameFields", new MerchantNameFields(context) }
            };
        }

        public override TriggerStatus OnAfterUpdate(List<Merchant> oldList, List<Merchant> newList) {
            (handlers["nameFields"] as MerchantNameFields).UpdateMerchantNames(newList);

            return TriggerStatus.SUCCESS;
        }
    }
}
