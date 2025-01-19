using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Errors {
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;

        public IndexModel(KoinzContext context) {
            _context = context;
        }

        public IList<ErrorLog> ErrorLog { get;set; } = default!;

        public async Task OnGetAsync() {
            ErrorLog = await _context.Errors
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedDate)
                .Take(500)
                .ToListAsync();
        }
    }
}
