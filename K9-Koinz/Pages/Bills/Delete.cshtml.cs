using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Bills {
    public class DeleteModel : AbstractDeleteModel<Bill> {
        public DeleteModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Bill> QueryRecordAsync(Guid id) {
            return await _context.Bills
                .Include(bill => bill.RepeatConfig)
                .SingleOrDefaultAsync(bill => bill.Id == id);
        }
    }
}
