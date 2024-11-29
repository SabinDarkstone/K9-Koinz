using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers.Handlers.Bills;

namespace K9_Koinz.Triggers {
    public class BillTrigger : GenericTrigger<Bill>, ITrigger<Bill> {
        
        public BillTrigger(KoinzContext context) : base(context) {
            handlers = new Dictionary<string, Handlers.AbstractTriggerHandler<Bill>> {
                { "nameFields", new BillNameFields(context) },
                { "defaultValues", new BillDefaultValues(context) }
            };
        }

        public TriggerStatus OnAfterDelete(List<Bill> newList) {
            return TriggerStatus.NO_TRIGGER;
        }

        public TriggerStatus OnAfterInsert(List<Bill> newList) {
            return TriggerStatus.NO_TRIGGER;
        }

        public TriggerStatus OnAfterUpdate(List<Bill> oldList, List<Bill> newList) {
            return TriggerStatus.NO_TRIGGER;
        }

        public TriggerStatus OnBeforeDelete(List<Bill> oldList) {
            return TriggerStatus.NO_TRIGGER;
        }

        public TriggerStatus OnBeforeInsert(List<Bill> newList) {
            (handlers["nameFields"] as BillNameFields).UpdateRealtedNameFields(newList);
            (handlers["defaultValues"] as BillDefaultValues).SetDefaultValues(newList);

            return TriggerStatus.SUCCESS;
        }

        public TriggerStatus OnBeforeUpdate(List<Bill> oldList, List<Bill> newList) {
            (handlers["nameFields"] as BillNameFields).UpdateRealtedNameFields(newList);

            return TriggerStatus.SUCCESS;
        }
    }
}
