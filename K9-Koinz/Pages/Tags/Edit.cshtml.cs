using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Tags {
    public class EditModel : EditPageModel<Tag> {
        public EditModel(TagRepository repository, IDropdownPopulatorService dropdownService)
            : base(repository, dropdownService) { }
    }
}
