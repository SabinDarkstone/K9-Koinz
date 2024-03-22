using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class BillRepository : GenericRepository<Bill> {
        public BillRepository(KoinzContext context) : base(context) { }

        public async Task<IEnumerable<Bill>> GetByAccountId(Guid accountId) {
            return await _context.Bills
                .Where(bill => bill.AccountId == accountId)
                .ToListAsync();
        }

        public async Task<Bill> GetDetails(Guid id) {
            return await _context.Bills
                .Include(bill => bill.Transactions)
                .Include(bill => bill.RepeatConfig)
                .FirstOrDefaultAsync(trans => trans.Id == id);
        }

        public async Task<IEnumerable<Bill>> GetAllBillsAsync() {
            return (await _context.Bills
                .AsNoTracking()
                .Include(bill => bill.Account)
                .Include(bill => bill.RepeatConfig)
                .ToListAsync())
                .OrderBy(bill => bill.RepeatConfig.NextFiring)
                .ToList();
        }

        public async Task<IEnumerable<Bill>> GetBillsWithinDateRangeAsync(DateTime start, DateTime end) {
            return (await _context.Bills
                .AsNoTracking()
                .Include(bill => bill.Account)
                .Include(bill => bill.RepeatConfig)
                .ToListAsync())
                .Where(bill => bill.RepeatConfigId.HasValue)
                .Where(bill => bill.RepeatConfig.NextFiring.HasValue)
                .Where(bill => bill.RepeatConfig.NextFiring >= start)
                .Where(bill => bill.RepeatConfig.NextFiring <= end)
                .OrderBy(bill => bill.RepeatConfig.NextFiring)
                .ToList();
        }

        public async Task<IEnumerable<Bill>> GetByCategory(Guid categoryId) {
            return await _context.Bills
                .Where(bill => bill.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bill>> GetByMerchant(Guid merchantId) {
            return await _context.Bills
                .Where(bill => bill.MerchantId == merchantId)
                .ToListAsync();
        }

        public async Task<SelectList> GetForDropdown(Guid accountId) {
            return new SelectList(await _context.Bills
                .Where(bill => bill.AccountId == accountId)
                .OrderBy(bill => bill.Name)
                .ToListAsync(), nameof(Bill.Id), nameof(Bill.Name));
        }
    }
}
