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

namespace K9_Koinz.Pages.BudgetLines {
    public class EditModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(KoinzContext context, ILogger<EditModel> logger) {
            _context = context;
            _logger = logger;
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
                .FirstAsync(line => line.Id == id);
            if (budgetLine == null) {
                return NotFound();
            }

            ViewData["BudgetId"] = new SelectList(_context.Budgets, nameof(Budget.Id), nameof(Budget.Name));
            ViewData["CategoryId"] = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));

            BudgetLine = budgetLine;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }
            _logger.LogInformation(BudgetLine.BudgetId.ToString());
            _context.Attach(BudgetLine).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!BudgetLineExists(BudgetLine.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("/Budgets/Edit", new { id = BudgetLine.BudgetId });
        }

        public IActionResult OnGetCategoryAutoComplete(string text) {
            text = text.Trim();
            var categories = _context.Categories
                .Include(cat => cat.ParentCategory)
                .AsNoTracking()
                .AsEnumerable()
                .Where(cat => cat.FullyQualifiedName.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(cat => new {
                    label = cat.ParentCategoryId != null ? cat.ParentCategory.Name + ": " + cat.Name : cat.Name,
                    val = cat.Id
                }).ToList();
            return new JsonResult(categories);
        }

        private bool BudgetLineExists(Guid id) {
            return _context.BudgetLines.Any(e => e.Id == id);
        }
    }
}
