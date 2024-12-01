using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Accounts {
    public class UpdateAccountNameFields : IHandler<Account> {
        private readonly KoinzContext _context;
        public UpdateAccountNameFields(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Account> oldList, List<Account> newList) {
            var accountIds = newList.Select(acct => acct.Id).ToList();

            var transactionsWithAccount = _context.Transactions
                .Where(trans => accountIds.Contains(trans.AccountId))
                .ToList();

            var billsWithAccount = _context.Bills
                .Where(bill => accountIds.Contains(bill.AccountId))
                .ToList();

            var goalsWithAccount = _context.SavingsGoals
                .Where(goal => accountIds.Contains(goal.AccountId))
                .ToList();

            var accountDict = newList.ToDictionary(acct => acct.Id, acct => acct);

            foreach (var t in transactionsWithAccount) {
                t.AccountName = accountDict[t.AccountId].Name;
            }

            foreach (var b in billsWithAccount) {
                b.AccountName = accountDict[b.AccountId].Name;
            }

            foreach (var g in goalsWithAccount) {
                g.AccountName = accountDict[g.AccountId].Name;
            }

            _context.Transactions.UpdateRange(transactionsWithAccount);
            _context.Bills.UpdateRange(billsWithAccount);
            _context.SavingsGoals.UpdateRange(goalsWithAccount);
        }
    }
}
