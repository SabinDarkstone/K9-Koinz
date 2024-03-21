using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Tags {
    public class IndexModel : AbstractDbPage {
        public IndexModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public IList<Tag> Tags { get; set; } = default!;

        public async Task OnGetAsync() {
            Tags = await _context.Tags.ToListAsync();
        }
    }
}
