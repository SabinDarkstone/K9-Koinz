using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class BillRepository : Repository<Bill> {
        public BillRepository(KoinzContext context) : base(context) { }

        public async Task<SelectList> GetBillOptions(Guid accountId) {
            return new SelectList(await _context.Bills
                .Where(bill => bill.AccountId == accountId)
                .OrderBy(bill => bill.Name)
                .ToListAsync(), nameof(Bill.Id), nameof(Bill.Name));
        }
    }
}
