using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Pages.BudgetLines {
    public class DeleteModel : PageModel {
        private readonly KoinzContext _context;

        public DeleteModel(KoinzContext context) {
            _context = context;
        }

        [BindProperty]
        public BudgetLine BudgetLine { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var budgetLine = await _context.BudgetLines
                .Include(line => line.BudgetCategory)
                .Include(line => line.Budget)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (budgetLine == null) {
                return NotFound();
            } else {
                BudgetLine = budgetLine;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var budgetLine = await _context.BudgetLines.FindAsync(id);
            if (budgetLine != null) {
                BudgetLine = budgetLine;
                _context.BudgetLines.Remove(BudgetLine);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Budgets/Edit", new { id = budgetLine.BudgetId });
        }
    }
}
