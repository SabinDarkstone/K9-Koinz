using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Data {
    public interface IBillRepository : IGenericRepository<Bill> {
        Task<IEnumerable<Bill>> GetAllBillsAsync();
        Task<IEnumerable<Bill>> GetBillsWithinDateRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<Bill>> GetByAccountId(Guid accountId);
        Task<IEnumerable<Bill>> GetByCategory(Guid categoryId);
        Task<IEnumerable<Bill>> GetByMerchant(Guid merchantId);
        Task<Bill> GetDetails(Guid id);
        Task<SelectList> GetForDropdown(Guid accountId);
    }
}