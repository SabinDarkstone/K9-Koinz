using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Newtonsoft.Json;
using System.Text.Json;

namespace K9_Koinz.Pages.Errors {
    public class DetailsModel : PageModel {
        private readonly KoinzContext _context;

        public DetailsModel(KoinzContext context) {
            _context = context;
        }

        public ErrorLog ErrorLog { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var errorlog = await _context.Errors.FirstOrDefaultAsync(m => m.Id == id);
            Exception ex = JsonConvert.DeserializeObject<Exception>(errorlog.ExceptionString);
            errorlog.ExceptionString = JsonConvert.SerializeObject(ex, Formatting.Indented);

            if (errorlog is not null) {
                ErrorLog = errorlog;

                return Page();
            }

            return NotFound();
        }
    }
}
