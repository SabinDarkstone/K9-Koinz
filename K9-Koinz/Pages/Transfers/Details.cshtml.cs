using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transfers {
    [Authorize]
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
                .Include(fer => fer.SavingsGoal)
                .Include(fer => fer.Transactions)
                    .ThenInclude(trans => trans.SplitTransactions)
                .FirstOrDefaultAsync(fer => fer.Id == id);
        }
    }
}
