using K9_Koinz.Data.Repositories.Meta;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class SavingsRepository : Repository<SavingsGoal> {
        public SavingsRepository(KoinzContext context) : base(context) { }

        public async Task<SelectList> GetGoalOptions(Guid? accountId = null) {
            if (accountId.HasValue) {
                return new SelectList(await _dbSet
                    .Where(goal => goal.AccountId == accountId.Value)
                    .ToListAsync(), nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
            } else {
                return new SelectList(
                    await GetAllAsync(),
                    nameof(SavingsGoal.Id), nameof(SavingsGoal.Name)
                );
            }
        }
    }
}
