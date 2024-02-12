using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Categories {
    public class DetailsModel : AbstractDetailsModel<Category> {
        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<Category> QueryRecordAsync(Guid id) {
            return await _context.Categories
                .Include(cat => cat.ParentCategory)
                .Include(cat => cat.ChildCategories)
                .AsNoTracking()
                .FirstOrDefaultAsync(cat => cat.Id == id);
        }
    }
}
