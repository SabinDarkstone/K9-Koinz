using K9_Koinz.Data.Repositories.Meta;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data.Repositories {
    public class TagRepository : Repository<Tag> {
        public TagRepository(KoinzContext context) : base(context) { }

        public async Task<Tag> GetTagAndTransactionsById(Guid tagId) {
            return await _dbSet.AsNoTracking()
                .Include(tag => tag.Transactions.OrderByDescending(trans => trans.Date))
                .FirstOrDefaultAsync(tag => tag.Id == tagId);
        }
    }
}
