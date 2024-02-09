using K9_Koinz.Data;
using K9_Koinz.Services.Meta;

namespace K9_Koinz.Services {
    public class BillService : AbstractService<BillService>, ICustomService {
        public BillService(KoinzContext context, ILogger<BillService> logger) : base(context, logger) { }
    }
}
