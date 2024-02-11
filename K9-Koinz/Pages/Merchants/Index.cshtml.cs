using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Merchants {
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;

        public IndexModel(KoinzContext context) {
            _context = context;
        }

        public IList<Merchant> Merchants { get; set; } = default!;

        public async Task OnGetAsync() {
            Merchants = await _context.Merchants
                .Include(merch => merch.Transactions)
                .OrderBy(merch => merch.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
