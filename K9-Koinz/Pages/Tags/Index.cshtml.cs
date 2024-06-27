using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using System.ComponentModel;

namespace K9_Koinz.Pages.Tags {
    public class IndexModel : AbstractDbPage {
        public IndexModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        [DisplayName("Show All Tags")]
        public bool ShowAllTags { get; set; }

        public IList<Tag> Tags { get; set; } = default!;

        public async Task OnGetAsync(string viewAll) {
            if (viewAll == "yes") {
                ShowAllTags = true;
            } else {
                ShowAllTags = false;
            }

            Tags = await _context.Tags.ToListAsync();
        }
    }
}
