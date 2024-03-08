using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Categories {
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;

        public IndexModel(KoinzContext context) {
            _context = context;
        }

        public IList<Category> Categories { get; set; } = default!;

        public async Task OnGetAsync() {
            var catList = await _context.Categories
                .AsNoTracking()
                .Include(cat => cat.ChildCategories)
                    .ThenInclude(cCat => cCat.Transactions)
                .Include(cat => cat.Transactions)
                .OrderBy(cat => cat.CategoryType)
                    .ThenBy(cat => cat.Name)
                .ToListAsync();

            Categories = catList
                .Where(cat => !cat.IsChildCategory)
                .ToList();
        }
    }
}
