using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Tags {
    public class DetailsModel : AbstractDetailsModel<Tag> {
        public DetailsModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Tag> QueryRecordAsync(Guid id) {
            return await _context.Tags
                .Include(tag => tag.Transactions)
                .FirstOrDefaultAsync(tag => tag.Id == id);
        }
    }
}
