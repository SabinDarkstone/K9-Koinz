using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers.Handlers.Budgets;

namespace K9_Koinz.Triggers {
    public class BudgetTrigger : GenericTrigger<Budget> {
        public BudgetTrigger(KoinzContext context) : base(context) { }

        public override TriggerStatus OnBeforeInsert(List<Budget> newList) {
            new SetBudgetNameFields(context).Execute(null, newList);

            return TriggerStatus.SUCCESS;
        }
    }
}
