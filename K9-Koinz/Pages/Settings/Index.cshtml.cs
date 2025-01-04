using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.Settings {
    public class IndexModel : PageModel {
        private readonly K9_Koinz.Data.KoinzContext _context;

        public IndexModel(K9_Koinz.Data.KoinzContext context)
        {
            _context = context;
        }

        public IList<Setting> Setting { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Setting = await _context.Settings.ToListAsync();
        }
    }
}
