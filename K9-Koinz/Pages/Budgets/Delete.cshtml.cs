using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.Budgets
{
    public class DeleteModel : PageModel {
        private readonly KoinzContext _context;

        public DeleteModel(KoinzContext context) {
            _context = context;
        }

        [BindProperty]
        public Budget Budget { get; set; } = default!;
        public List<BudgetLine> BudgetLines { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var budget = await _context.Budgets.FirstOrDefaultAsync(m => m.Id == id);

            if (budget == null) {
                return NotFound();
            } else {
                Budget = budget;
            }

            var unsortedLines = await _context.BudgetLines
                .Where(line => line.BudgetId == Budget.Id)
                .Include(line => line.BudgetCategory)
                .ToListAsync();
            BudgetLines = unsortedLines.SortCategories();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var budget = await _context.Budgets.FindAsync(id);
            if (budget != null) {
                Budget = budget;
                _context.Budgets.Remove(Budget);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
