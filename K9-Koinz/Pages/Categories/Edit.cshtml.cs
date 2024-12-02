using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Categories {
    public class EditModel : EditPageModel<Category> {
        public EditModel(CategoryRepository repository, IDropdownPopulatorService dropdownService)
            : base(repository, dropdownService) { }

        protected override async Task<Category> QueryRecord(Guid id) {
            return await (_repository as CategoryRepository).GetCategoryWithFamily(id);
        }
    }
}
