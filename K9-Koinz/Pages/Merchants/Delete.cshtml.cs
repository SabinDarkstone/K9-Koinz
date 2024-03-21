using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Merchants {
    public class DeleteModel : AbstractDeleteModel<Merchant> {
        public DeleteModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Merchant> QueryRecordAsync(Guid id) {
            return await _context.Merchants
                .Include(merch => merch.Transactions)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
