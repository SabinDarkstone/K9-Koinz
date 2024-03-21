using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transfers.Recurring {
    public class DeleteModel : AbstractDeleteModel<Transfer> {
        public DeleteModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Transfer> QueryRecordAsync(Guid id) {
            return await _context.Transfers
                .Include(fer => fer.Tag)
                .Include(fer => fer.Category)
                .Include(fer => fer.FromAccount)
                .Include(fer => fer.ToAccount)
                .Include(fer => fer.Merchant)
                .Include(fer => fer.RepeatConfig)
                .Include(fer => fer.SavingsGoal)
                .FirstOrDefaultAsync(fer => fer.Id == id);
        }
    }
}
