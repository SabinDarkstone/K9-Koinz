using K9_Koinz.Data;
using K9_Koinz.Services.Meta;

namespace K9_Koinz.Services {
    public class DbCleanupService : AbstractService<DbCleanupService>, ICustomService {
        public DbCleanupService(KoinzContext context, ILogger<DbCleanupService> logger) : base(context, logger) { }
    }
}
