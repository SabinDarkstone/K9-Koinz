using Microsoft.EntityFrameworkCore;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data;
using Microsoft.AspNetCore.Authorization;

namespace K9_Koinz.Pages.Merchants {
    [Authorize]
    public class DetailsModel : AbstractDetailsModel<Merchant> {
        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public List<Transaction> Transactions { get; set; }

        protected override async Task AdditionalActionsAsync() {
            Transactions = await _context.Transactions
                .AsNoTracking()
                .Where(trans => trans.MerchantId == Record.Id)
                .OrderByDescending(trans => trans.Date)
                .ToListAsync();
        }
    }
}
