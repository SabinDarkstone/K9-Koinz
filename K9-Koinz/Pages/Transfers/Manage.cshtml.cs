using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.Transfers {
    public class ManageModel : AbstractDbPage {
        public ManageModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public Dictionary<string, List<Transfer>> RecurringTransfersDict;

        public async Task<IActionResult> OnGetAsync() {
            RecurringTransfersDict = await _data.Transfers.GetRecurringGroupedByAccount();

            return Page();
        }
    }
}
