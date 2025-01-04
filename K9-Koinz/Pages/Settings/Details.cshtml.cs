using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Settings
{
    public class DetailsModel : PageModel
    {
        private readonly K9_Koinz.Data.KoinzContext _context;

        public DetailsModel(K9_Koinz.Data.KoinzContext context)
        {
            _context = context;
        }

        public Setting Setting { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings.FirstOrDefaultAsync(m => m.Id == id);

            if (setting is not null)
            {
                Setting = setting;

                return Page();
            }

            return NotFound();
        }
    }
}
