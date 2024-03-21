using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class TransactionRepository : GenericRepository<Transaction> {
        public TransactionRepository(KoinzContext context)
            : base(context) { }

        public double GetTransactionTotalSinceBalanceSet(Account account) {
            var runningTotal = _context.Transactions
                .Where(trans => trans.Date > account.InitialBalanceDate || (trans.Date.Date == account.InitialBalanceDate.Date && trans.DoNotSkip))
                .Where(trans => trans.AccountId == account.Id).GetTotal();

            return runningTotal;
        }

        public async Task<IEnumerable<Transaction>> GetByAccountId(Guid accountId) {
            return await _context.Transactions
                .Where(trans => trans.AccountId == accountId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetForSpendingHistory(Guid categoryId) {
            return await _context.Transactions
                .Include(trans => trans.Category)
                .AsNoTracking()
                .Where(trans => trans.CategoryId == categoryId || trans.Category.ParentCategoryId == categoryId)
                .Where(trans => !trans.IsSavingsSpending)
                .Where(trans => !trans.IsSplit)
                .Where(trans => trans.Date <= DateTime.Today.Date.Date && trans.Date.Date >= DateTime.Today.AddMonths(-11))
                .ToListAsync();
        }

        public async Task<Transaction> GetSplitLines(Guid parentId) {
            return await _context.Transactions
                .Include(trans => trans.SplitTransactions
                    .OrderBy(splt => splt.CategoryName))
                .Where(trans => trans.Id == parentId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public bool AnyInMonth(DateTime refDate) {
            return _context.Transactions
                .AsNoTracking()
                .Any(trans => trans.Date >= refDate.StartOfMonth() && trans.Date <= refDate.EndOfMonth());
        }

        public bool AnyInWeek(DateTime refDate) {
            return _context.Transactions
                .AsNoTracking()
                .Any(trans => trans.Date >= refDate.StartOfWeek() && trans.Date <= refDate.EndOfWeek());
        }

        public bool AnyInYear(DateTime refDate) {
            return _context.Transactions
                .AsNoTracking()
                .Any(trans => trans.Date >= refDate.StartOfYear() && trans.Date <= refDate.EndOfYear());
        }

        public Transaction GetMatchingFromTransferPair(Guid transferId, Guid transactionId) {
            return _context.Transactions
                .Where(trans => trans.TransferId == transferId)
                .Where(trans => trans.Id != transactionId)
                .SingleOrDefault();
        }

        public Transaction GetWithCategory(Guid id) {
            return _context.Transactions
                .Include(trans => trans.Category)
                .Where(trans => trans.Id == id)
                .SingleOrDefault();
        }

        public 
    }
}
