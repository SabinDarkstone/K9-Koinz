using K9_Koinz.Data;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Controllers {
    public class AutocompleteController : GenericController {
        public AutocompleteController(IRepositoryWrapper data)
            : base(data) { }

        public async Task<JsonResult> GetAutocompleteCategoriesAsync(string text) {
            var suggestions = await _data.Categories.GetForAutocomplete(text);
            return new JsonResult(suggestions);
        }

        public async Task<JsonResult> GetAutocompleteMerchantsAsync(string text) {
            var suggestions = await _data.Merchants.GetForAutocomplete(text);
            return new JsonResult(suggestions);
        }

        public async Task<JsonResult> GetSuggestedCategoryAsync(string merchantId) {
            var transaction = await _data.Transactions
                .GetTransFromMostPopularCategoryByMerchant(merchantId);
            var category = await _data.Categories.GetByIdAsync(transaction.CategoryId);

            if (category != null) {
                return new JsonResult(category);
            } else {
                return null;
            }
        }

        public async Task<JsonResult> OnGetParentCategoryAutoComplete(string text) {
            var categories = await _data.Categories.GetParentCategoryAutocomplete(text);
            return new JsonResult(categories);
        }
    }
}
