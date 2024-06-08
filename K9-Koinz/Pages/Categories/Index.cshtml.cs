using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using System.ComponentModel;

namespace K9_Koinz.Pages.Categories {
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;

        [DisplayName("Show All Categories")]
        public bool ShowAllCategories { get; set; } = false;

        public IndexModel(KoinzContext context) {
            _context = context;
        }

        public IList<Category> Categories { get; set; } = default!;

        public async Task OnGetAsync(string viewAll) {
            if (viewAll == "yes") {
                ShowAllCategories = true;
            } else {
                ShowAllCategories = false;
            }

            IQueryable<Category> categoriesIQ = _context.Categories
                .AsNoTracking()
                .Include(cat => cat.ChildCategories)
                    .ThenInclude(cCat => cCat.Transactions)
                .Include(cat => cat.Transactions)
                .OrderBy(cat => cat.CategoryType)
                    .ThenBy(cat => cat.Name);

            if (!ShowAllCategories) {
                categoriesIQ = categoriesIQ.Where(cat => !cat.IsRetired);
            }
            var catList = await categoriesIQ.ToListAsync();

            Categories = catList
                .Where(cat => !cat.IsChildCategory)
                .ToList();
        }
    }
}
