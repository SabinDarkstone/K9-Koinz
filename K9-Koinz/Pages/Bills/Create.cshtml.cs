using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Triggers;

namespace K9_Koinz.Pages.Bills {
    public class CreateModel : AbstractCreateModel<Bill> {
        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger, dropdownService) {
            trigger = new BillTrigger(context);
        }
    }
}
