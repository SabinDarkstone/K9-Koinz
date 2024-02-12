using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Bills {
    public class DetailsModel : AbstractDetailsModel<Bill> {
        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<Bill> QueryRecordAsync(Guid id) {
            return await _context.Bills
                .Include(bill => bill.Transactions)
                .FirstOrDefaultAsync(trans => trans.Id == id);
        }
    }
}
