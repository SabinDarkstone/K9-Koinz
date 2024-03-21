using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Tags {
    public class EditModel : AbstractEditModel<Tag> {
        public EditModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
            : base(data, logger, dropdownService) { }
    }
}
