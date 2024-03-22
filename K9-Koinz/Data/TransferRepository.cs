using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class TransferRepository : GenericRepository<Transfer>, ITransferRepository {
        public TransferRepository(KoinzContext context) : base(context) { }

        public async Task<Dictionary<string, List<Transfer>>> GetRecurringGroupedByAccount() {
            return await DbSet
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
            return await DbSet
                .Include(fer => fer.FromAccount)
                .Include(fer => fer.ToAccount)
                .Include(fer => fer.RepeatConfig)
                .Include(fer => fer.Merchant)
                .Include(fer => fer.Category)
                .Include(fer => fer.SavingsGoal)
                .Include(fer => fer.Tag)
                .Include(fer => fer.Transactions
                    .OrderByDescending(trans => trans.Date)
                    .Take(100))
                .SingleOrDefaultAsync(fer => fer.Id == id);
        }

        public async Task<IEnumerable<Transfer>> FindDuplicates(Transfer original) {
            return (await DbSet
                .Where(fer => fer.ToAccountId == original.ToAccountId && fer.FromAccountId == original.FromAccountId)
                .Where(fer => fer.Amount == original.Amount)
                .Where(fer => fer.RepeatConfig.FirstFiring == original.RepeatConfig.FirstFiring)
                .Where(fer => fer.RepeatConfig.Mode == original.RepeatConfig.Mode)
                .Where(fer => fer.RepeatConfig.IntervalGap == original.RepeatConfig.IntervalGap)
                .Where(fer => fer.RepeatConfig.Frequency == original.RepeatConfig.Frequency)
                .Where(fer => fer.Id != original.Id)
                .ToListAsync())
                .Where(fer => Math.Abs((fer.Date - original.Date).TotalDays) <= 5)
                .ToList();
        }

        public IEnumerable<Transfer> GetRecurringBeforeDate(DateTime mark) {
            return DbSet
                .Where(fer => fer.RepeatConfigId.HasValue)
                .Include(fer => fer.RepeatConfig)
                .AsEnumerable()
                .Where(fer => fer.RepeatConfig.NextFiring.HasValue)
                .Where(fer => fer.RepeatConfig.NextFiring.Value.Date <= mark.Date)
                .ToList();
        }
    }
}