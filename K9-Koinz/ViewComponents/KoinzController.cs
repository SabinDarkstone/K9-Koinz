using K9_Koinz.Data;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.ViewComponents {
    public abstract class KoinzController<TController> : ViewComponent {
        protected readonly KoinzContext _context;
        protected readonly ILogger<TController> _logger;

        public KoinzController(KoinzContext context, ILogger<TController> logger) {
            _context = context;
            _logger = logger;
        }
    }
}
