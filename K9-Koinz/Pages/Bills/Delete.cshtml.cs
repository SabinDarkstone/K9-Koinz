using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Bills {
    public class DeleteModel : PageModel {
        private readonly KoinzContext _context;

        public DeleteModel(KoinzContext context) {
            _context = context;
        }

        [BindProperty]
        public Bill Bill { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(bill => bill.Transactions)
                .FirstOrDefaultAsync(bill => bill.Id == id);

            if (bill == null) {
                return NotFound();
            } else {
                Bill = bill;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var bill = await _context.Bills.FindAsync(id);
            if (bill != null) {
                Bill = bill;
                _context.Bills.Remove(Bill);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
