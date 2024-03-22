using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Data {
    public interface ICategoryRepository : IGenericRepository<Category> {
        Task<IEnumerable<Category>> GetAll();
        Category GetByName(string name);
        Task<Category> GetCategoryDetails(Guid id);
        Task<IEnumerable<Category>> GetChildrenAsync(Guid parentCategoryId);
        Task<SelectList> GetDropdown();
        Task<IEnumerable<AutocompleteResult>> GetForAutocomplete(string searchText);
        Task<IEnumerable<AutocompleteResult>> GetParentCategoryAutocomplete(string searchText);
    }
}