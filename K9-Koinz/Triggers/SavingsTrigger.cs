using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers.Handlers.Savings;

namespace K9_Koinz.Triggers {
    public class SavingsTrigger : GenericTrigger<SavingsGoal> {
        public SavingsTrigger(KoinzContext context) : base(context) { }

        public override TriggerStatus OnBeforeUpdate(List<SavingsGoal> oldList, List<SavingsGoal> newList) {
            new UpdateSavingsNameFields(context).Execute(oldList, newList);
            return TriggerStatus.SUCCESS;
        }

        public override TriggerStatus OnBeforeInsert(List<SavingsGoal> newList) {
            new UpdateSavingsNameFields(context).Execute(null, newList);
            return TriggerStatus.SUCCESS;
        }

        public override TriggerStatus OnAfterUpdate(List<SavingsGoal> oldList, List<SavingsGoal> newList) {
            new UpdateSavingsTransfers(context).Execute(oldList, newList);
            return TriggerStatus.SUCCESS;
        }
    }
}
