using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers.Handlers.Bills;

namespace K9_Koinz.Triggers {
    public class BillTrigger : GenericTrigger<Bill> {
        public BillTrigger(KoinzContext context) : base(context) { }

        public override TriggerStatus OnBeforeInsert(List<Bill> newList) {
            new SetBillDefaultValues(context).Execute(null, newList);
            new SetBillNameFields(context).Execute(null, newList);

            return TriggerStatus.SUCCESS;
        }

        public override TriggerStatus OnBeforeUpdate(List<Bill> oldList, List<Bill> newList) {
            new SetBillNameFields(context).Execute(oldList, newList);

            return TriggerStatus.SUCCESS;
        }
    }
}
