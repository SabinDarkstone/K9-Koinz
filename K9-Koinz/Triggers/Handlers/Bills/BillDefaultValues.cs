using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Bills {
    public class BillDefaultValues : AbstractTriggerHandler<Bill> {
        public BillDefaultValues(KoinzContext context) : base(context) { }

        public void SetDefaultValues(List<Bill> newList) {
            foreach (var bill in newList) {
                if (bill.RepeatConfig == null) {
                    throw new Exception("RepeatConfig must be included in query when inserting a new bill");
                }

                bill.IsActive = true;
            }
        }
    }
}
