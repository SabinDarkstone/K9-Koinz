using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace K9_Koinz.Triggers.Handlers.Transactions {
    public class TransactionTransferPair {
        public static void UpdateOtherTransaction(KoinzContext context, ModelStateDictionary modelState, List<Transaction> newList) {
            // This stores key/value pairs of transfer Id to transactions
            Dictionary<Guid, Transaction> transactionDict = new();

            HashSet<Guid> transactionIds = newList.Select(trans => trans.Id).ToHashSet();

            foreach (var transaction in newList) {
                if (transaction.TransferId != null) {
                    transactionDict.Add(transaction.TransferId.Value, transaction);
                }
            }

            if (transactionDict.Count == 0) {
                return;
            }

            var otherTransactionDict = context.Transactions
                .Where(trans => trans.TransferId != null)
                .Where(trans => !transactionIds.Contains(trans.Id))
                .Where(trans => transactionDict.Keys.Contains(trans.TransferId.Value))
                .ToDictionary(trans => trans.TransferId, trans => trans);

            List<Transaction> transactionsToUpdate = new();
            foreach (var transferId in transactionDict.Keys) {
                Transaction transPair = new();
                Transaction original = transactionDict[transferId];

                var ferSuccess = otherTransactionDict.TryGetValue2(transferId, out transPair);
                if (ferSuccess == Status.SUCCESS) {
                    transPair.Amount = -1 * original.Amount;
                    transPair.Date = original.Date;
                    transPair.Notes = original.Notes;
                    transPair.TagId = original.TagId;
                    transactionsToUpdate.Add(transPair);
                }
            }

            context.Transactions.UpdateRange(transactionsToUpdate);
        }

        public static void DeleteOtherTransaction(KoinzContext context, ModelStateDictionary modelState, List<Transaction> oldList) {
            HashSet<Guid> transferIds = new();
            HashSet<Guid> transactionIds = oldList.Select(trans => trans.Id).ToHashSet();

            foreach (var transaction in oldList) {
                if (transaction.TransferId != null) {
                    transferIds.Add(transaction.TransferId.Value);
                }
            }

            if (transferIds.Count == 0) {
                return;
            }

            var otherTransactions = context.Transactions
                .Where(trans => !transactionIds.Contains(trans.Id))
                .Where(trans => trans.TransferId != null)
                .Where(trans => transferIds.Contains(trans.TransferId.Value))
                .ToList();

            context.Transactions.RemoveRange(otherTransactions);
        }
    }
}
