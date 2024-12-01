using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers.Handlers.Transactions;

namespace K9_Koinz.Triggers {
    public class TransactionTrigger : GenericTrigger<Transaction> {
        public TransactionTrigger(KoinzContext context) : base(context) { }

        public override TriggerStatus OnBeforeInsert(List<Transaction> newList) {
            new TransactionNameFields(context).Execute(null, newList);
            new TransactionDateField(context).Execute(null, newList);

            return TriggerStatus.SUCCESS;
        }

        public override TriggerStatus OnBeforeUpdate(List<Transaction> oldList, List<Transaction> newList) {
            new TransactionNameFields(context).Execute(null, newList);
            new TransactionDateField(context).Execute(null, newList);

            return TriggerStatus.SUCCESS;
        }

        public override TriggerStatus OnAfterUpdate(List<Transaction> oldList, List<Transaction> newList) {
            new UpdateGoalAmounts(context).Execute(oldList, newList);
            new UpdateTransactionPair(context).Execute(oldList, newList);
            new UpdateSplitTransactions(context).Execute(oldList, newList);

            return TriggerStatus.SUCCESS;
        }

        public override TriggerStatus OnBeforeDelete(List<Transaction> oldList) {
            new DeleteSplitTransactions(context).Execute(oldList, null);

            return TriggerStatus.SUCCESS;
        }

        public override TriggerStatus OnAfterDelete(List<Transaction> oldList) {
            new RemoveTransactionFromGoal(context).Execute(oldList, null);
            new DeleteTransactionPair(context).Execute(oldList, null);

            return TriggerStatus.SUCCESS;
        }
    }
}
