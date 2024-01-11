using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transactions {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(KoinzContext context, ILogger<CreateModel> logger) {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet() {
            ViewData["AccountId"] = new SelectList(_context.Accounts.OrderBy(acct => acct.Name), nameof(Account.Id), nameof(Account.Name));
            ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(cat => cat.Name), nameof(Category.Id), nameof(Category.Name));
            ViewData["MerchantId"] = new SelectList(_context.Merchants.OrderBy(merch => merch.Name), nameof(Merchant.Id), nameof(Merchant.Name));
            return Page();
        }

        [BindProperty]
        public Transaction Transaction { get; set; } = default!;
        public List<Merchant> Merchants { get; set; }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            _context.Transactions.Add(Transaction);
            _logger.LogInformation(Transaction.MerchantId.ToString());
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public IActionResult OnGetAutoComplete(string text) {
            var merchants = _context.Merchants.Where(merch => merch.Name.StartsWith(text)).Select(merch => new {
                label = merch.Name,
                val = merch.Id
            }).ToList();
            return new JsonResult(merchants);
        }
    }
}
