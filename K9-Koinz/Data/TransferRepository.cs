using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class TransferRepository : GenericRepository<Transfer> {
        public TransferRepository(KoinzContext context) : base(context) { }

        public async Task<Dictionary<string, List<Transfer>>> GetRecurringGroupedByAccount() {
            return await _context.Transfers
                .Include(fer => fer.FromAccount)
                .Include(fer => fer.ToAccount)
                .Include(fer => fer.RepeatConfig)
                .Include(fer => fer.Merchant)
                .Include(fer => fer.Category)
                .Include(fer => fer.SavingsGoal)
                .Where(fer => fer.RepeatConfigId.HasValue)
                .AsNoTracking()
                .GroupBy(fer => fer.FromAccount.Name)
                .ToDictionaryAsync(
                    x => x.Key,
                    x => x.AsEnumerable()
                        .OrderBy(fer => fer.RepeatConfig.NextFiring)
                        .ThenBy(fer => fer.ToAccount.Name)
                        .ToList()
                );
        }

        public async Task<Transfer> GetDetails(Guid id) {
            return await _context.Transfers
                .Include(fer => fer.FromAccount)
                .Include(fer => fer.ToAccount)
                .Include(fer => fer.RepeatConfig)
                .Include(fer => fer.Merchant)
                .Include(fer => fer.Category)
                .Include(fer => fer.SavingsGoal)
                .SingleOrDefaultAsync(fer => fer.Id == id);
        }
    }
}