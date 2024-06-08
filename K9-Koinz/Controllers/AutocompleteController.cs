using K9_Koinz.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Controllers {
    public class AutocompleteController : Controller {
        private readonly KoinzContext _context;

        public AutocompleteController(KoinzContext context) {
            _context = context;
        }

        public async Task<JsonResult> GetAutocompleteCategoriesAsync(string text) {
            var suggestions = (await _context.Categories
                .Where(cat => !cat.IsRetired)
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

        public async Task<JsonResult> GetAutocompleteMerchantsAsync(string text) {
            var suggestions = (await _context.Merchants
                .Where(merch => !merch.IsRetired)
                .AsNoTracking()
                .ToListAsync())
                .Where(merch => merch.Name != null && merch.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(merch => new {
                    label = merch.Name,
                    val = merch.Id
                }).ToList();

            return new JsonResult(suggestions);
        }

        public async Task<JsonResult> GetSuggestedCategoryAsync(string merchantId) {
            var transactionsByCategory = (await _context.Transactions
                .AsNoTracking()
                .Where(trans => trans.MerchantId == Guid.Parse(merchantId))
                .ToListAsync())
                .GroupBy(x => x.CategoryId)
                .OrderByDescending(x => x.ToList().Count)
                .FirstOrDefault();

            // Get the most commonly used category
            var category = await _context.Categories.FindAsync(transactionsByCategory.ToList().FirstOrDefault().CategoryId);

            if (category != null) {
                return new JsonResult(category);
            } else {
                return null;
            }
        }

        public async Task<JsonResult> OnGetParentCategoryAutoComplete(string text) {
            text = text.Trim();
            var categories = (await _context.Categories
                .Include(cat => !cat.IsChildCategory)
                .AsNoTracking()
                .ToListAsync())
                .Where(cat => cat.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(cat => new {
                    label = cat.Name,
                    val = cat.Id
                }).ToList();
            return new JsonResult(categories);
        }
    }
}
