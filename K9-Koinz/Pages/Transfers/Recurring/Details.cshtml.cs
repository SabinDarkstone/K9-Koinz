using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transfers.Recurring {
    public class DetailsModel : AbstractDetailsModel<Transfer> {
        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<Transfer> QueryRecordAsync(Guid id) {
            return await _context.Transfers
                .Include(fer => fer.Tag)
                .Include(fer => fer.Category)
                .Include(fer => fer.FromAccount)
                .Include(fer => fer.ToAccount)
                .Include(fer => fer.Merchant)
                .Include(fer => fer.RepeatConfig)
                .Include(fer => fer.SavingsGoal)
                .Include(fer => fer.Transactions)
                .FirstOrDefaultAsync(fer => fer.Id == id);
        }
    }
}
