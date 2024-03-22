using K9_Koinz.Models;

namespace K9_Koinz.Data {
    public interface IBudgetLineRepository : IGenericRepository<BudgetLine> {
        Task<IEnumerable<BudgetLine>> GetByBudget(Guid budgetId);
        Task<IEnumerable<BudgetLine>> GetByCategory(Guid categoryId);
        Task<BudgetLine> GetByIdAsync(Guid? id);
    }
}