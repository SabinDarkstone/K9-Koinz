using K9_Koinz.Models;
using K9_Koinz.Triggers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data.Repositories {
    public class BillRepository : TriggeredRepository<Bill> {
        public BillRepository(KoinzContext context, ITrigger<Bill> trigger) : base(context, trigger) { }

        public async Task<SelectList> GetBillOptions(Guid accountId) {
            return new SelectList(await _context.Bills
                .Where(bill => bill.AccountId == accountId)
                .OrderBy(bill => bill.Name)
                .ToListAsync(), nameof(Bill.Id), nameof(Bill.Name));
        }
    }
}
