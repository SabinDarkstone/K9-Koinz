using K9_Koinz.Data;
using K9_Koinz.Services.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Services {
    public interface IAutocompleteService : ICustomService {
        public abstract Task<JsonResult> AutocompleteCategoriesAsync(string text);
        public abstract Task<JsonResult> AutocompleteMerchantsAsync(string text);
    }

    public class AutocompleteService : AbstractService<AutocompleteService>, IAutocompleteService {
        public AutocompleteService(KoinzContext context, ILogger<AutocompleteService> logger) : base(context, logger) { }

        public async Task<JsonResult> AutocompleteCategoriesAsync(string text) {
            var suggestions = (await _context.Categories
                .Include(cat => cat.ParentCategory)
                .AsNoTracking()
                .ToListAsync())
                .Where(cat => cat.FullyQualifiedName.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(cat => new {
                    label = cat.ParentCategoryId != null ? cat.ParentCategory.Name + ": " + cat.Name : cat.Name,
                    val = cat.Id
                }).ToList();
            return new JsonResult(suggestions);
        }

        public async Task<JsonResult> AutocompleteMerchantsAsync(string text) {
            var suggestions = (await _context.Merchants
                .AsNoTracking()
                .ToListAsync())
                .Where(merch => merch.Name != null && merch.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(merch => new {
                    label = merch.Name,
                    val = merch.Id
                }).ToList();

            return new JsonResult(suggestions);
        }
    }
}
