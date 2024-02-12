using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Merchants {
    public class DeleteModel : AbstractDeleteModel<Merchant> {
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public override async Task<IActionResult> OnGetAsync(Guid? id, bool? saveChangesError = false) {
            if (id == null) {
                return NotFound();
            }

            var merchant = await _context.Merchants
                .Include(merch => merch.Transactions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (merchant == null) {
                return NotFound();
            }

            Record = merchant;

            if (saveChangesError.GetValueOrDefault()) {
                ErrorMessage = string.Format("Delete {id} failed. Try again.", id);
            }

            return Page();
        }
    }
}
