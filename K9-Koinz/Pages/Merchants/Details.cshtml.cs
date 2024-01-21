using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Merchants {
    public class DetailsModel : PageModel {
        private readonly KoinzContext _context;

        public DetailsModel(KoinzContext context) {
            _context = context;
        }

        public Merchant Merchant { get; set; } = default!;
        public List<Transaction> Transactions { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var merchant = await _context.Merchants
                .AsNoTracking()
                .FirstOrDefaultAsync(merch => merch.Id == id);

            if (merchant == null) {
                return NotFound();
            } else {
                Merchant = merchant;
                Transactions = await _context.Transactions
                    .Where(trans => trans.MerchantId == Merchant.Id)
                    .OrderByDescending(trans => trans.Date)
                    .ToListAsync();
            }
            return Page();
        }
    }
}
