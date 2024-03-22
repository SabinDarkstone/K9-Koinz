using K9_Koinz.Models;

namespace K9_Koinz.Data {
    public interface IJobStatusRepository : IGenericRepository<ScheduledJobStatus> {
        Task<IEnumerable<ScheduledJobStatus>> GetAllAsync();
    }
}