using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Triggers;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Bills {
    public class EditModel : AbstractEditModel<Bill> {
        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IDropdownPopulatorService dropdownService)
            : base(context, logger, dropdownService) {
            trigger = new BillTrigger(context);
        }

        protected override async Task<Bill> QueryRecordAsync(Guid id) {
            return await _context.Bills
                .Include(bill => bill.RepeatConfig)
                .SingleOrDefaultAsync(bill => bill.Id == id);
        }
    }
}
