using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using Microsoft.AspNetCore.Authorization;

namespace K9_Koinz.Pages.Merchants {
    [Authorize]
    public class CreateModel : AbstractCreateModel<Merchant> {
        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
                : base(context, logger, dropdownService) { }
    }
}
