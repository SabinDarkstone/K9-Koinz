using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Merchants {
    [Authorize]
    public class EditModel : AbstractEditModel<Merchant> {
        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger, dropdownService) { }

        protected override async Task AfterSaveActionsAsync() {
            var relatedTransactions = await _context.Transactions
                .Where(trans => trans.MerchantId == Record.Id)
                .ToListAsync();

            var relatedBills = await _context.Bills
                .Where(bill => bill.MerchantId == Record.Id)
                .ToListAsync();

            foreach (var trans in relatedTransactions) {
                trans.MerchantName = Record.Name;
            }

            foreach (var bill in relatedBills) {
                bill.MerchantName = Record.Name;
            }

            _context.Transactions.UpdateRange(relatedTransactions);
            _context.Bills.UpdateRange(relatedBills);
            await _context.SaveChangesAsync();
        }
    }
}
