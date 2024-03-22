using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class JobStatusRepository : GenericRepository<ScheduledJobStatus>, IJobStatusRepository {
        public JobStatusRepository(KoinzContext context) : base(context) { }

        public async Task<IEnumerable<ScheduledJobStatus>> GetAllAsync() {
            return await DbSet
                .AsNoTracking()
                .OrderByDescending(job => job.StartTime)
                .ToListAsync();
        }
    }
}