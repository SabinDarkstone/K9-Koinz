using K9_Koinz.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace K9_Koinz.Pages.Meta {
    public abstract class AbstractDbPage : PageModel {
        protected readonly KoinzContext _context;
        protected ILogger<AbstractDbPage> _logger;

        protected AbstractDbPage(KoinzContext context, ILogger<AbstractDbPage> logger) {
            _context = context;
            _logger = logger;
        }
    }
}
