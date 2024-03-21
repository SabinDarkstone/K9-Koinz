using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Categories {
    public class DeleteModel : AbstractDeleteModel<Category> {
        public DeleteModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Category> QueryRecordAsync(Guid id) {
            return await _context.Categories
                .Include(cat => cat.ChildCategories)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        protected override void AdditioanlDatabaseActions() {
            _context.Categories.RemoveRange(Record.ChildCategories);
        }
    }
}
