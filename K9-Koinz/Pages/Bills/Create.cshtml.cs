using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Bills {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;

        public CreateModel(KoinzContext context) {
            _context = context;
        }

        public IActionResult OnGet() {
            AccountOptions = AccountUtils.GetAccountList(_context, true);

            return Page();
        }

        [BindProperty]
        public Bill Bill { get; set; } = default!;
        public List<SelectListItem> AccountOptions;

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var account = _context.Accounts.Find(Bill.AccountId);
            var merchant = _context.Merchants.Find(Bill.MerchantId);

            Bill.AccountName = account.Name;
            Bill.MerchantName = merchant.Name;

            _context.Bills.Add(Bill);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public IActionResult OnGetMerchantAutoComplete(string text) {
            text = text.Trim();
            var merchants = _context.Merchants
                .AsNoTracking()
                .AsEnumerable()
                .Where(merch => merch.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(merch => new {
                    label = merch.Name,
                    val = merch.Id
                }).ToList();
            return new JsonResult(merchants);
        }
    }
}
