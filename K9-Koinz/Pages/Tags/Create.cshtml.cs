using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Tags {
    public class CreateModel : CreatePageModel<Tag> {
        public CreateModel(TagRepository repository, IDropdownPopulatorService dropdownService)
            : base(repository, dropdownService) { }
    }
}
