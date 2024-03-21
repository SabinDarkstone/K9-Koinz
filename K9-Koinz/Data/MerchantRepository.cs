using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class MerchantRepository : GenericRepository<Merchant> {
        public MerchantRepository(KoinzContext context) : base(context) { }

        public async Task<Merchant> GetDetailsAsync(Guid id) {
            return await _context.Merchants
                .Include(merch => merch.Transactions
                    .Take(100)
                    .OrderByDescending(trans => trans.Date))
                .AsNoTracking()
                .Where(merch => merch.Id == id)
                .SingleOrDefaultAsync();
        }
        
        public async Task<IEnumerable<Merchant>> GetAll() {
            return await _context.Merchants
                .Include(merch => merch.Transactions)
                .OrderBy(merch => merch.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}