using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Transactions {
    public class DetailsModel : PageModel {
        private readonly KoinzContext _context;

        public DetailsModel(KoinzContext context) {
            _context = context;
        }

        public Transaction Transaction { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(trans => trans.Merchant)
                .Include(trans => trans.Account)
                .Include(trans => trans.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null) {
                return NotFound();
            } else {
                Transaction = transaction;
            }
            return Page();
        }
    }
}
