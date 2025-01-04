using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Settings
{
    public class CreateModel : PageModel
    {
        private readonly K9_Koinz.Data.KoinzContext _context;

        public CreateModel(K9_Koinz.Data.KoinzContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Setting Setting { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Settings.Add(Setting);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
