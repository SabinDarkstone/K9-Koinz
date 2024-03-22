using K9_Koinz.Models;

namespace K9_Koinz.Data {
    public interface ITransferRepository : IGenericRepository<Transfer> {
        Task<IEnumerable<Transfer>> FindDuplicates(Transfer original);
        Task<Transfer> GetDetails(Guid id);
        IEnumerable<Transfer> GetRecurringBeforeDate(DateTime mark);
        Task<Dictionary<string, List<Transfer>>> GetRecurringGroupedByAccount();
    }
}