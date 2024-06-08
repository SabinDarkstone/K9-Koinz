using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using System.ComponentModel;

namespace K9_Koinz.Pages.Merchants {
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;

        public IndexModel(KoinzContext context) {
            _context = context;
        }

        [DisplayName("Show All Merchants")]
        public bool ShowAllMerchants { get; set; }

        public IList<Merchant> Merchants { get; set; } = default!;

        public async Task OnGetAsync(string viewAll) {
            if (viewAll == "yes") {
                ShowAllMerchants = true;
            } else {
                ShowAllMerchants = false;
            }

            IQueryable<Merchant> merchantsIQ = _context.Merchants
                .AsNoTracking()
                .Include(merch => merch.Transactions)
                .OrderBy(merch => merch.Name);

            if (!ShowAllMerchants) {
                merchantsIQ = merchantsIQ.Where(merch => !merch.IsRetired);
            }

            Merchants = await merchantsIQ.ToListAsync();
        }
    }
}
