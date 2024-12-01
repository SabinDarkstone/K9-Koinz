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

        public async Task<Bill> GetBillWithDetails(Guid billId, bool doTracking = false) {
            IQueryable<Bill> iq = _dbSet.Include(bill => bill.RepeatConfig);

            if (!doTracking) {
                iq = iq.AsNoTracking();
            }

            return await iq.FirstOrDefaultAsync(bill => bill.Id == billId);
        }

        public List<Transaction> GetTransactionsForBill(Guid billId) {
            return _context.Transactions
                .AsNoTracking()
                .Where(trans => trans.BillId == billId)
                .ToList();
        }

        public async Task<List<Bill>> GetBillList(bool includeAll) {
            List<Bill> bills;

            IQueryable<Bill> iq = _dbSet.AsNoTracking()
                .Include(bill => bill.RepeatConfig)
                .Include(bill => bill.Account);

            if (includeAll) {
                bills = await iq.AsSplitQuery()
                    .ToListAsync();
            } else {
                bills = await iq.AsSplitQuery()
                    .Where(bill => bill.IsActive)
                    .ToListAsync();
            }

            bills = bills.OrderBy(bill => bill.RepeatConfig.CalculatedNextFiring).ToList();
            return bills;
        }
    }
}
