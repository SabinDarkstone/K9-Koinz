using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Categories {
    public class CreateModel : CreatePageModel<Category> {
        public Category ParentCategory { get; set; }

        public CreateModel(CategoryRepository repository, IDropdownPopulatorService dropdownService)
            : base(repository, dropdownService) { }

        protected override async Task OnPageLoadActionsAsync() {
            ParentCategory = await (_repository as CategoryRepository).GetParentCategory(RelatedId);
        }
    }
}
