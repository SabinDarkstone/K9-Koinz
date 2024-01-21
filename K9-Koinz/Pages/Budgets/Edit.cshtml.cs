using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.Budgets {
    public class EditModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(KoinzContext context, ILogger<EditModel> logger) {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Budget Budget { get; set; } = default!;
        public SelectList TagOptions;

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            TagOptions = TagUtils.GetTagList(_context);

            var budget = await _context.Budgets
                .Include(bud => bud.BudgetLines)
                    .ThenInclude(line => line.BudgetCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budget == null) {
                return NotFound();
            }
            Budget = budget;
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

            _context.Attach(Budget).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!BudgetExists(Budget.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BudgetExists(Guid id) {
            return _context.Budgets.Any(e => e.Id == id);
        }
    }
}
