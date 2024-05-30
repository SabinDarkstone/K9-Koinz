using Microsoft.EntityFrameworkCore;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data;
using Microsoft.AspNetCore.Authorization;

namespace K9_Koinz.Pages.JobStatuses {
    [Authorize]
    public class IndexModel : AbstractDbPage {
        public IndexModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public IList<ScheduledJobStatus> ScheduledJobStatus { get; set; } = default!;

        public async Task OnGetAsync() {
            ScheduledJobStatus = await _context.JobStatuses
                .AsNoTracking()
                .OrderByDescending(job => job.StartTime)
                .ToListAsync();
        }
    }
}
