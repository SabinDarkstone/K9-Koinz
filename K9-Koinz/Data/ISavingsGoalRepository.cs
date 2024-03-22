using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Data {
    public interface ISavingsGoalRepository : IGenericRepository<SavingsGoal> {
        bool ExistsByAccountId(Guid accountId);
        Task<Dictionary<string, List<SavingsGoal>>> GetAllGroupedByAccount();
        Task<SavingsGoal> GetDetailsAsync(Guid id);
        SelectList GetForDropdown(Guid? accountId);
    }
}