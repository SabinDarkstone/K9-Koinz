using K9_Koinz.Models;

namespace K9_Koinz.Data {
    public interface IBudgetRepository : IGenericRepository<Budget> {
        Task<IEnumerable<Budget>> GetAllAsync();
        Budget GetBudgetDetails(string budgetId);
        Task<Budget> GetBudgetDetailsShorter(Guid budgetId);
    }
}