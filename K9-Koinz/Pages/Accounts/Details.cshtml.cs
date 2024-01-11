using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Accounts {
    public class DetailsModel : PageModel {
        private readonly KoinzContext _context;

        public DetailsModel(KoinzContext context) {
            _context = context;
        }

        public Account Account { get; set; } = default!;
        public List<Transaction> Transactions => Account?.Transactions.ToList() ?? new List<Transaction>();

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(acct => acct.Transactions)
                .ThenInclude(trans => trans.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(acct => acct.Id == id);

            if (account == null) {
                return NotFound();
            } else {
                Account = account;
            }
            return Page();
        }
    }
}
