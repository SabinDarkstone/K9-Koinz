using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Services {
    public interface IAutocompleteService : ICustomService {
        public abstract JsonResult AutocompleteCategories(string text);
        public abstract JsonResult AutocompleteMerchants(string text);
    }

    public class AutocompleteService : AbstractService<AutocompleteService>, IAutocompleteService {
        public AutocompleteService(KoinzContext context, ILogger<AutocompleteService> logger) : base(context, logger) { }

        public JsonResult AutocompleteCategories(string text) {
            var suggestions = _context.Categories
                .Include(cat => cat.ParentCategory)
                .AsNoTracking()
                .AsEnumerable()
                .Where(cat => cat.FullyQualifiedName.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(cat => new {
                    label = cat.ParentCategoryId != null ? cat.ParentCategory.Name + ": " + cat.Name : cat.Name,
                    val = cat.Id
                }).ToList();
            return new JsonResult(suggestions);
        }

        public JsonResult AutocompleteMerchants(string text) {
            var suggestions = _context.Merchants
                .AsNoTracking()
                .AsEnumerable()
                .Where(merch => merch.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(merch => new {
                    label = merch.Name,
                    val = merch.Id
                }).ToList();

            return new JsonResult(suggestions);
        }
    }
}
