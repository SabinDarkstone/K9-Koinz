using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Triggers.Handlers.Bills;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace K9_Koinz.Triggers {
    public class BillTrigger : GenericTrigger<Bill>, ITrigger<Bill> {
        
        public BillTrigger(KoinzContext context, ILogger logger) : base(context, logger) {
            handlers = new Dictionary<string, Handlers.AbstractTriggerHandler<Bill>> {
                { "nameFields", new BillNameFields(context, logger) },
                { "defaultValues", new BillDefaultValues(context, logger) }
            };
        }

        public void SetState(ModelStateDictionary state) {
            foreach (var handler in handlers.Values) {
                handler.SetModelState(state);
            }
        }

        public void OnAfterDelete(List<Bill> newList) {
        }

        public void OnAfterInsert(List<Bill> newList) {
        }

        public void OnAfterUpdate(List<Bill> oldList, List<Bill> newList) {
        }

        public void OnBeforeDelete(List<Bill> oldList) {
        }

        public void OnBeforeInsert(List<Bill> newList) {
            (handlers["nameFields"] as BillNameFields).UpdateRealtedNameFields(newList);
            (handlers["defaultValues"] as BillDefaultValues).SetDefaultValues(newList);
        }

        public void OnBeforeUpdate(List<Bill> oldList, List<Bill> newList) {
            (handlers["nameFields"] as BillNameFields).UpdateRealtedNameFields(newList);
        }
    }
}
