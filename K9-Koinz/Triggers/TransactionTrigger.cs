using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers.Handlers;
using K9_Koinz.Triggers.Handlers.Transactions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace K9_Koinz.Triggers {
    public class TransactionTrigger : GenericTrigger<Transaction>, ITrigger<Transaction> {
        public TransactionTrigger(KoinzContext context) : base(context) {
            handlers = new Dictionary<string, AbstractTriggerHandler<Transaction>> {
                { "nameFields", new TransactionNameFields(context) },
                { "dateFields", new TransactionDateField(context) },
                { "goalUpdate", new TransactionGoalUpdate(context) },
                { "splitLines", new TransactionSplitLines(context) },
                { "transferPair", new TransactionTransferPair(context) }
            };
        }

        public TriggerStatus OnBeforeInsert(List<Transaction> newList) {
            (handlers["nameFields"] as TransactionNameFields).UpdateRelatedNameFields(newList);
            (handlers["dateFields"] as TransactionDateField).UpdateDateField(newList);

            return TriggerStatus.SUCCESS;
        }

        public TriggerStatus OnAfterInsert(List<Transaction> newList) {
            return TriggerStatus.NO_TRIGGER;
        }

        public TriggerStatus OnBeforeUpdate(List<Transaction> oldList, List<Transaction> newList) {
            (handlers["nameFields"] as TransactionNameFields).UpdateRelatedNameFields(newList);
            (handlers["dateFields"] as TransactionDateField).UpdateDateField(newList);

            return TriggerStatus.SUCCESS;
        }

        public TriggerStatus OnAfterUpdate(List<Transaction> oldList, List<Transaction> newList) {
            (handlers["goalUpdate"] as TransactionGoalUpdate).UpdateGoalForUpdate(oldList, newList);
            (handlers["transferPair"] as TransactionTransferPair).UpdateOtherTransaction(newList);
            (handlers["splitLines"] as TransactionSplitLines).UpdateSplitChildren(oldList, newList);

            return TriggerStatus.SUCCESS;
        }

        public TriggerStatus OnBeforeDelete(List<Transaction> oldList) {
            (handlers["splitLines"] as TransactionSplitLines).DeleteSplitChildren(oldList);

            return TriggerStatus.SUCCESS;
        }

        public TriggerStatus OnAfterDelete(List<Transaction> oldList) {
            (handlers["goalUpdate"] as TransactionGoalUpdate).UpdateGoalForDelete(oldList);
            (handlers["transferPair"] as TransactionTransferPair).DeleteOtherTransaction(oldList);

            return TriggerStatus.SUCCESS;
        }
    }
}
