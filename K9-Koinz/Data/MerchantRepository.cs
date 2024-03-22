using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace K9_Koinz.Data {
    public class MerchantRepository : GenericRepository<Merchant> {
        public MerchantRepository(KoinzContext context) : base(context) { }

        public async Task<Merchant> GetDetailsAsync(Guid id) {
            return await DbSet
                .Include(merch => merch.Transactions
                    .Take(100)
                    .OrderByDescending(trans => trans.Date))
                .AsNoTracking()
                .Where(merch => merch.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Merchant>> GetAll() {
            return await DbSet
                .Include(merch => merch.Transactions)
                .OrderBy(merch => merch.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<AutocompleteResult>> GetForAutocomplete(string searchText) {
            return (await DbSet
                .AsNoTracking()
                .ToListAsync())
                .Where(merch => merch.Name != null && merch.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                .Select(merch => new AutocompleteResult {
                    Label = merch.Name,
                    Id = merch.Id
                }).ToList();
        }
    }
}