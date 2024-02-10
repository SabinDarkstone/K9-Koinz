using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Merchants {
    public class EditModel : AbstractEditModel<Merchant> {
        public EditModel(KoinzContext context, IAccountService accountService, IAutocompleteService autocompleteService, ITagService tagService)
            : base(context, accountService, autocompleteService, tagService) { }

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var merchant = await _context.Merchants.FirstOrDefaultAsync(m => m.Id == id);
            if (merchant == null) {
                return NotFound();
            }
            Record = merchant;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            _context.Attach(Record).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!RecordExists(Record.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
