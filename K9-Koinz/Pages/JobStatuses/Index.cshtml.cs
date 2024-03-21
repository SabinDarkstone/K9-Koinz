using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data;

namespace K9_Koinz.Pages.JobStatuses {
    public class IndexModel : AbstractDbPage {
        public IndexModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public IEnumerable<ScheduledJobStatus> ScheduledJobStatus { get; set; } = default!;

        public async Task OnGetAsync() {
            ScheduledJobStatus = await _data.JobStatusRepository.GetAllAsync();
    }
}
