using K9_Koinz.Models;
using K9_Koinz.Triggers;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data.Repositories {
    public class MerchantRepository : TriggeredRepository<Merchant> {
        public MerchantRepository(KoinzContext context, ITrigger<Merchant> trigger) : base(context, trigger) {
        }

        public async Task<Merchant> GetMerchantWithTransactionsById(Guid merchantId) {
            return await _dbSet
                .Include(merch => merch.Transactions)
                .FirstOrDefaultAsync(m => m.Id == merchantId);
        }

        public async Task<double> GetAverageSpending(DateTime startDate, DateTime endDate, Guid merchantId) {
            var transactions = await _context.Transactions.AsNoTracking()
                .Include(trans => trans.Category)
                .Where(trans => trans.CategoryId.HasValue && trans.TransferId == null)
                .Where(trans => !trans.IsSplit && !trans.IsSavingsSpending)
                .Where(trans => trans.MerchantId == merchantId)
                .Where(trans => trans.Date.Date >= startDate.Date && trans.Date.Date <= endDate.Date)
                .ToListAsync();

            if (!transactions.Any()) {
                return 0;
            }

            return (transactions.GroupBy(trans => trans.Date.Month)
                .ToDictionary(x => x.Key, x => x.Sum(trans => trans.Amount)))
                .Values.Average();
        }
    }
}
