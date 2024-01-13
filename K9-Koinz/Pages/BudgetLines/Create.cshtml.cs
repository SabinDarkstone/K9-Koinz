using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.BudgetLines {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;

        public Budget Budget { get; set; }

        [BindProperty]
        public BudgetLine BudgetLine { get; set; }

        public CreateModel(KoinzContext context) {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(Guid budgetId) {
            Budget = await _context.Budgets.FindAsync(budgetId);

            ViewData["CategoryId"] = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name)).OrderBy(cat => cat.Text);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            _context.BudgetLines.Add(BudgetLine);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Budgets/Edit", new { id = BudgetLine.BudgetId });
        }

        public IActionResult OnGetCategoryAutoComplete(string text) {
            text = text.Trim();
            var categories = _context.Categories
                .Include(cat => cat.ParentCategory)
                .AsNoTracking()
                .AsEnumerable()
                .Where(cat => cat.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase) || (cat.ParentCategoryId.HasValue && cat.ParentCategory.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase)))
                .Select(cat => new {
                    label = cat.ParentCategoryId != null ? cat.ParentCategory.Name + ": " + cat.Name : cat.Name,
                    val = cat.Id
                }).ToList();
            return new JsonResult(categories);
        }
    }
}
