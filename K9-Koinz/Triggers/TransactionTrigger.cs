using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Triggers.Handlers.Transactions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace K9_Koinz.Triggers
{
    public class TransactionTrigger : ITrigger<Transaction> {

        private readonly KoinzContext _context;
        private ModelStateDictionary modelState;

        public TransactionTrigger(KoinzContext context) {
            _context = context;
        }

        public void SetState(ModelStateDictionary modelState) {
            this.modelState = modelState;
        }

        public void OnBeforeInsert(List<Transaction> newList) {
            TransactionNameFields.UpdateRelatedNameFields(_context, modelState, newList);
            TransactionDateField.UpdateDateField(modelState, newList);
        }

        public void OnAfterInsert(List<Transaction> newList) {

        }

        public void OnBeforeUpdate(List<Transaction> oldList, List<Transaction> newList) {
            TransactionNameFields.UpdateRelatedNameFields(_context, modelState, newList);
            TransactionDateField.UpdateDateField(modelState, newList);
        }

        public void OnAfterUpdate(List<Transaction> oldList, List<Transaction> newList) {

        }

        public void OnBeforeDelete(List<Transaction> oldList) {

        }

        public void OnAfterDelete(List<Transaction> oldList) {

        }
    }
}
