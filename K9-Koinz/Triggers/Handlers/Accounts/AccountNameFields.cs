using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Accounts {
    public class AccountNameFields : AbstractTriggerHandler<Account> {
        public AccountNameFields(KoinzContext context) : base(context) { }

        public void UpdateAccountNames(List<Account> accountList) {
            var accountIds = accountList.Select(acct => acct.Id).ToList();

            var transactionsWithAccount = context.Transactions
                .Where(trans => accountIds.Contains(trans.AccountId))
                .ToList();

            var billsWithAccount = context.Bills
                .Where(bill => accountIds.Contains(bill.AccountId))
                .ToList();

            var goalsWithAccount = context.SavingsGoals
                .Where(goal => accountIds.Contains(goal.AccountId))
                .ToList();

            var accountDict = accountList.ToDictionary(acct => acct.Id, acct => acct);

            foreach (var t in transactionsWithAccount) {
                t.AccountName = accountDict[t.AccountId].Name;
            }

            foreach (var b in billsWithAccount) {
                b.AccountName = accountDict[b.AccountId].Name;
            }

            foreach (var g in goalsWithAccount) {
                g.AccountName = accountDict[g.AccountId].Name;
            }

            context.Transactions.UpdateRange(transactionsWithAccount);
            context.Bills.UpdateRange(billsWithAccount);
            context.SavingsGoals.UpdateRange(goalsWithAccount);
        }
    }
}
