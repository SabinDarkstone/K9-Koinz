using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Accounts {
    public class EditModel : AbstractEditModel<Account> {
        public EditModel(KoinzContext context, IAccountService accountService, IAutocompleteService autocompleteService, ITagService tagService)
            : base(context, accountService, autocompleteService, tagService) { }

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == id);
            if (account == null) {
                return NotFound();
            }
            Record = account;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            Record.InitialBalanceDate = Record.InitialBalanceDate.AtMidnight();

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
