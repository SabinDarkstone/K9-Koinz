using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Triggers.Handlers;
using K9_Koinz.Triggers.Handlers.Transactions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace K9_Koinz.Triggers {
    public class TransactionTrigger : GenericTrigger<Transaction>, ITrigger<Transaction> {
        public TransactionTrigger(KoinzContext context, ILogger logger) : base(context, logger) {
            handlers = new Dictionary<string, AbstractTriggerHandler<Transaction>> {
                { "nameFields", new TransactionNameFields(context, logger) },
                { "dateFields", new TransactionDateField(context, logger) },
                { "goalUpdate", new TransactionGoalUpdate(context, logger) },
                { "splitLines", new TransactionSplitLines(context, logger) },
                { "transferPair", new TransactionTransferPair(context, logger) }
            };
        }

        public void SetState(ModelStateDictionary state) {
            foreach (var handler in handlers.Values) {
                handler.SetModelState(state);
            }
        }

        public void OnBeforeInsert(List<Transaction> newList) {
            (handlers["nameFields"] as TransactionNameFields).UpdateRelatedNameFields(newList);
            (handlers["dateFields"] as TransactionDateField).UpdateDateField(newList);
        }

        public void OnAfterInsert(List<Transaction> newList) {

        }

        public void OnBeforeUpdate(List<Transaction> oldList, List<Transaction> newList) {
            (handlers["nameFields"] as TransactionNameFields).UpdateRelatedNameFields(newList);
            (handlers["dateFields"] as TransactionDateField).UpdateDateField(newList);
        }

        public void OnAfterUpdate(List<Transaction> oldList, List<Transaction> newList) {
            (handlers["goalUpdate"] as TransactionGoalUpdate).UpdateGoalForUpdate(oldList, newList);
            (handlers["transferPair"] as TransactionTransferPair).UpdateOtherTransaction(newList);
            (handlers["splitLines"] as TransactionSplitLines).UpdateSplitChildren(oldList, newList);
        }

        public void OnBeforeDelete(List<Transaction> oldList) {
            (handlers["splitLines"] as TransactionSplitLines).DeleteSplitChildren(oldList);
        }

        public void OnAfterDelete(List<Transaction> oldList) {
            (handlers["goalUpdate"] as TransactionGoalUpdate).UpdateGoalForDelete(oldList);
            (handlers["transferPair"] as TransactionTransferPair).DeleteOtherTransaction(oldList);
        }
    }
}
