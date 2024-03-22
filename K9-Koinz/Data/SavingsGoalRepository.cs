using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class SavingsGoalRepository : GenericRepository<SavingsGoal>, ISavingsGoalRepository {
        public SavingsGoalRepository(KoinzContext context) : base(context) { }

        public async Task<SavingsGoal> GetDetailsAsync(Guid id) {
            return await DbSet
                .Include(goal => goal.Transactions)
                .Where(goal => goal.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public async Task<Dictionary<string, List<SavingsGoal>>> GetAllGroupedByAccount() {
            return await DbSet
                .Include(goal => goal.Transactions)
                .AsNoTracking()
                .GroupBy(goal => goal.AccountName)
                .ToDictionaryAsync(
                    x => x.Key,
                    x => x.AsEnumerable().OrderBy(goal => goal.Name).ToList()
                );
        }

        public SelectList GetForDropdown(Guid? accountId) {
            if (accountId.HasValue) {
                return new SelectList(DbSet
                    .Where(goal => goal.AccountId == accountId.Value)
                    .OrderBy(goals => goals.Name)
                    .ToList(), nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
            } else {
                return new SelectList(DbSet
                    .OrderBy(goal => goal.Name)
                    .ToList(), nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
            }
        }

        public bool ExistsByAccountId(Guid accountId) {
            return DbSet
                .Any(goal => goal.AccountId == accountId);
        }
    }
}