using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.Accounts {
    public class DetailsModel : AbstractDetailsModel<Account> {
        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public List<Transaction> Transactions => Record?.Transactions
            .OrderByDescending(trans => trans.Date)
            .ToList() ?? new List<Transaction>();

        protected override async Task<Account> QueryRecordAsync(Guid id) {
            return await _context.Accounts
                .Include(acct => acct.Transactions)
                    .ThenInclude(trans => trans.Category)
                .Include(acct => acct.Transactions)
                    .ThenInclude(trans => trans.Merchant)
                .AsNoTracking()
                .FirstOrDefaultAsync(acct => acct.Id == id);
        }

        protected override void AdditionalActions() {
            var newBalance = Transactions
                .Where(trans => trans.Date > Record.InitialBalanceDate || (trans.Date.Date == Record.InitialBalanceDate.Date && trans.DoNotSkip))
                .Where(trans => trans.AccountId == Record.Id).GetTotal();
            Record.CurrentBalance = Record.InitialBalance + newBalance;
        }
    }
}
