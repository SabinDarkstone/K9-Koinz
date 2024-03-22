using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data;

namespace K9_Koinz.Pages.JobStatuses {
    public class IndexModel : AbstractIndexModel<ScheduledJobStatus> {
        public IndexModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public async Task OnGetAsync() {
            RecordList = await _data.JobStatusRepository.GetAllAsync();
        }
    }
}