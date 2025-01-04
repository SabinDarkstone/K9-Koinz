using K9_Koinz.Data;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Services {
    public class SettingsService {
        private readonly KoinzContext _context;

        public SettingsService(KoinzContext context) {
            _context = context;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync() {
            return await _context.Settings.ToDictionaryAsync(x => x.Name, x => x.Value);
        }
    }
}
