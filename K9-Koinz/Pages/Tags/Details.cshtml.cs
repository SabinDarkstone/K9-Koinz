using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Authorization;

namespace K9_Koinz.Pages.Tags {
    [Authorize]
    public class DetailsModel : AbstractDetailsModel<Tag> {
        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<Tag> QueryRecordAsync(Guid id) {
            return await _context.Tags
                .Include(tag => tag.Transactions)
                .FirstOrDefaultAsync(tag => tag.Id == id);
        }
    }
}
