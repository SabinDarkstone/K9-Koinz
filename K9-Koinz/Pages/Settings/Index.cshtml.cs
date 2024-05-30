using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace K9_Koinz.Pages.Settings {
    public class IndexModel : PageModel {
        [Authorize]
        public void OnGet() {
        }
    }
}
