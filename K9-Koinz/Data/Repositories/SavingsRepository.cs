using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Triggers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class SavingsRepository : TriggeredRepository<SavingsGoal> {
        public SavingsRepository(KoinzContext context, ITrigger<SavingsGoal> trigger) : base(context, trigger) { }

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
