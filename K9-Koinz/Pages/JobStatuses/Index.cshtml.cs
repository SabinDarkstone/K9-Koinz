using Microsoft.EntityFrameworkCore;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data;

namespace K9_Koinz.Pages.JobStatuses {
    public class IndexModel : AbstractDbPage {
        public IndexModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public IList<ScheduledJobStatus> ScheduledJobStatus { get; set; } = default!;

        public async Task OnGetAsync() {
            ScheduledJobStatus = await _context.JobStatuses
                .OrderByDescending(job => job.StartTime)
                .ToListAsync();
        }
    }
}
