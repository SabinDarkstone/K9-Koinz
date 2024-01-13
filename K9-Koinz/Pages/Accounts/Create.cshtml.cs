using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;

namespace K9_Koinz.Pages.Accounts {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;

        public CreateModel(KoinzContext context) {
            _context = context;
        }

        public IActionResult OnGet() {
            return Page();
        }

        [BindProperty]
        public Account Account { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            Account.InitialBalanceDate = Account.InitialBalanceDate.AtMidnight();

            _context.Accounts.Add(Account);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
