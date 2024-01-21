using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.Budgets {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;

        public CreateModel(KoinzContext context) {
            _context = context;
        }

        [BindProperty]
        public Budget Budget { get; set; } = default!;
        public SelectList TagOptions;

        public IActionResult OnGet() {
            TagOptions = TagUtils.GetTagList(_context);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            if (Budget.BudgetTagId == Guid.Empty) {
                Budget.BudgetTagId = null;
            } else {
                var tag = await _context.Tags.FindAsync(Budget.BudgetTagId);
                Budget.BudgetTagName = tag.Name;
            }

            _context.Budgets.Add(Budget);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
