using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Data {
    public class AccountRepository : GenericRepository<Account> {
        public AccountRepository(KoinzContext context) : base(context) { }

        public async Task<Account> GetAccountDetails(Guid accountId) {
            var account = await _context.Accounts
                .Include(acct => acct.Transactions
                    .Take(100)
                    .OrderByDescending(trans => trans.Date))
                .AsNoTracking()
                .SingleOrDefaultAsync(acct => acct.Id == accountId);

            return account;
        }

        public async Task<Dictionary<string, List<Account>>> GetAllGroupedByType() {
            return (await _context.Accounts
                .AsNoTracking()
                .ToListAsync())
                .GroupBy(acct => acct.Type.GetAttribute<DisplayAttribute>().Name)
                .ToDictionary(grp => grp.Key, grp => grp.OrderBy(acct => acct.Name).ToList());
        }

        public async Task<IEnumerable<Account>> GetAll() {
            return await _context.Accounts
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
