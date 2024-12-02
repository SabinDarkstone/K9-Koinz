using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Categories {
    public class DeleteModel : DeletePageModel<Category> {

        public DeleteModel(CategoryRepository repository) : base(repository) { }

        protected override async Task<Category> QueryRecord(Guid id) {
            var category = await _repository.GetByIdAsync(id);
            var childern = (_repository as CategoryRepository).GetChildCategories(id);
            category.ChildCategories = childern;
            return category;
        }
    }
}
