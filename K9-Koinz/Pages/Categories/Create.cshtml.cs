using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Categories {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;

        public CreateModel(KoinzContext context) {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;
        public Category ParentCategory { get; set; }

        public async Task<IActionResult> OnGet(Guid? parentCategoryId) {
            if (parentCategoryId.HasValue) {
                ParentCategory = await _context.Categories.FindAsync(parentCategoryId);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            if (Category.ParentCategoryId.HasValue) {
                var parentCategory = await _context.Categories.FindAsync(Category.ParentCategoryId);
                Category.ParentCategoryName = parentCategory.Name;
            }

            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        public IActionResult OnGetCategoryAutoComplete(string text) {
            text = text.Trim();
            var categories = _context.Categories
                .Where(cat => cat.ParentCategoryId == null)
                .AsNoTracking()
                .AsEnumerable()
                .Where(cat => cat.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(cat => new {
                    label = cat.Name,
                    val = cat.Id
                }).ToList();
            return new JsonResult(categories);
        }

    }
}
