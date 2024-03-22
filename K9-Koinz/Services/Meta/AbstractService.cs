using K9_Koinz.Data;

namespace K9_Koinz.Services.Meta {
    public abstract class AbstractService<T> where T : ICustomService {
        protected readonly KoinzContext _context;
        protected readonly ILogger<T> _logger;

        protected AbstractService(KoinzContext context, ILogger<T> logger) {
            _context = context;
            _logger = logger;
        }
    }
}
