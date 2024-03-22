using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class TagRepository : GenericRepository<Tag> {
        public TagRepository(KoinzContext context) : base(context) { }

        public async Task<Tag> GetDetails(Guid id) {
            return await DbSet
                .Include(tag => tag.Transactions
                    .OrderByDescending(trans => trans.Date)
                    .Take(100))
                .SingleOrDefaultAsync(tag => tag.Id == id);
        }

        public async Task<IEnumerable<Tag>> GetAll() {
            return await DbSet
                .OrderBy(tag => tag.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}