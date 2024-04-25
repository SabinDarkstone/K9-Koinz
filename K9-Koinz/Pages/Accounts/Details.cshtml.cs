using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Accounts {
    public class DetailsModel : AbstractDetailsModel<Account> {
        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public List<Transaction> Transactions { get; set; }

        protected override async Task<Account> QueryRecordAsync(Guid id) {
            Transactions = await _context.Transactions
                .AsNoTracking()
                .Include(trans => trans.ParentTransaction)
                .Include(trans => trans.SplitTransactions)
                .Include(trans => trans.Category)
                    .ThenInclude(cat => cat.ParentCategory)
                .Where(trans => trans.AccountId == id)
                .OrderByDescending(trans => trans.Date)
                .Take(100)
                .ToListAsync();

            return await _context.Accounts
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(acct => acct.Id == id);
        }

        protected override void AdditionalActions() {
            var newBalance = Transactions
                .Where(trans => trans.Date > Record.InitialBalanceDate || (trans.Date.Date == Record.InitialBalanceDate.Date && trans.DoNotSkip))
                .Where(trans => !trans.IsSplit)
                .Where(trans => trans.AccountId == Record.Id).GetTotal();
            Record.CurrentBalance = Record.InitialBalance + newBalance;
        }
    }
}
