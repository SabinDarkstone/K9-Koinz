using K9_Koinz.Models;

namespace K9_Koinz.Data {
    public interface IAccountRepository : IGenericRepository<Account> {
        Task<Account> GetAccountDetails(Guid accountId);
        Task<IEnumerable<Account>> GetAll();
        Task<Dictionary<string, List<Account>>> GetAllGroupedByType();
    }
}