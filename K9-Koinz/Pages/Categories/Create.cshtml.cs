using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Categories {
    public class CreateModel : AbstractCreateModel<Category> {
        public CreateModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(data, logger, dropdownService) { }

        public Category ParentCategory { get; set; }

        protected override async Task BeforePageLoadActions() {
            await base.BeforePageLoadActions();

            if (RelatedId.HasValue) {
                ParentCategory = await _data.CategoryRepository.GetByIdAsync(RelatedId.Value);
            }
        }

        protected override async Task BeforeSaveActionsAsync() {
            if (Record.ParentCategoryId.HasValue) {
                var parentCategory = await _data.CategoryRepository.GetByIdAsync(Record.ParentCategoryId.Value);
                Record.ParentCategoryName = parentCategory.Name;
            }
        }
    }
}
