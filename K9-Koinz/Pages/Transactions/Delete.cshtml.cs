using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transactions {
    public class DeleteModel : AbstractDeleteModel<Transaction> {
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public override async Task<IActionResult> OnGetAsync(Guid? id, bool? saveChangedError = false) {
            if (id == null) {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(trans => trans.Tag)
                .FirstOrDefaultAsync(trans => trans.Id == id);

            if (transaction == null) {
                return NotFound();
            } else {
                Record = transaction;
            }
            return Page();
        }
    }
}
