using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Accounts {
    public class EditModel : AbstractEditModel<Account> {
        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger, dropdownService) { }

        protected override void BeforeSaveActions() {
            Record.InitialBalanceDate = Record.InitialBalanceDate.AtMidnight();
        }

        protected override async Task AfterSaveActionsAsync() {
            var relatedTransactions = await _context.Transactions
                .Where(trans => trans.AccountId == Record.Id)
                .ToListAsync();

            var relatedBills = await _context.Bills
                .Where(bill => bill.AccountId == Record.Id)
                .ToListAsync();

            foreach (var trans in relatedTransactions) {
                trans.AccountName = Record.Name;
            }

            foreach (var bill in relatedBills) {
                bill.AccountName = Record.Name;
            }

            _context.Transactions.UpdateRange(relatedTransactions);
            _context.Bills.UpdateRange(relatedBills);
            await _context.SaveChangesAsync();
        }
    }
}
