using K9_Koinz.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace K9_Koinz.Pages.Meta {
    public abstract class AbstractDbPage : PageModel {
        protected readonly RepositoryWrapper _data;
        protected ILogger<AbstractDbPage> _logger;

        protected AbstractDbPage(RepositoryWrapper data, ILogger<AbstractDbPage> logger) {
            _data = data;
            _logger = logger;
        }
    }
}
