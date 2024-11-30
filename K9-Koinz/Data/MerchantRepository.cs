using K9_Koinz.Models;
using K9_Koinz.Triggers;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class MerchantRepository : TriggeredRepository<Merchant> {
        public MerchantRepository(KoinzContext context, ITrigger<Merchant> trigger) : base(context, trigger) {
        }

        public async Task<Merchant> GetMerchantWithTransactionsById(Guid merchantId) {
            return await _dbSet
                .Include(merch => merch.Transactions)
                .FirstOrDefaultAsync(m => m.Id == merchantId);
        }
    }
}
