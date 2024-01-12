using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Merchants {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;

        public CreateModel(KoinzContext context) {
            _context = context;
        }

        public IActionResult OnGet() {
            return Page();
        }

        [BindProperty]
        public Merchant Merchant { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            if (_context.Merchants.Any(merch => merch.Name == Merchant.Name)) {

            }

            _context.Merchants.Add(Merchant);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
