using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Triggers;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data.Repositories {
    public class AccountRepository : TriggeredRepository<Account> {
        public AccountRepository(KoinzContext context, ITrigger<Account> trigger)
            : base(context, trigger) { }

        public List<Transaction> GetRecentTransactionsForAccount(Guid accountId, int count) {
            return _context.Transactions
                .AsNoTracking()
                .Include(trans => trans.ParentTransaction)
                .Include(trans => trans.SplitTransactions)
                .Include(trans => trans.Category)
                    .ThenInclude(cat => cat.ParentCategory)
                .Where(trans => trans.AccountId == accountId)
                .OrderByDescending(trans => trans.Date)
                .Take(count)
                .ToList();
        }

        public double GetCurrentBalance(Guid accountId) {
            var account = _dbSet.AsNoTracking().FirstOrDefault(acct => acct.Id == accountId);
            return account.InitialBalance + _context.Transactions
                .AsNoTracking()
                .Where(trans => trans.AccountId == accountId)
                .Where(trans => trans.Date.Date > account.InitialBalanceDate.Date || (trans.Date.Date == account.InitialBalanceDate && trans.DoNotSkip))
                .Where(trans => !trans.IsSplit)
                .GetTotal();
        }
    }
}
