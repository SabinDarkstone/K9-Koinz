using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace K9_Koinz.Pages.Settings {
    [Authorize]
    public class ReloadCategoriesModel : PageModel {
        public void OnGet() {
        }
    }
}
