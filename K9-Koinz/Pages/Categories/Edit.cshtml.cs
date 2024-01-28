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
using Humanizer;

namespace K9_Koinz.Pages.Categories {
    public class EditModel : PageModel {
        private readonly KoinzContext _context;

        public EditModel(KoinzContext context) {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(cat => cat.ChildCategories)
                .Include(cat => cat.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) {
                return NotFound();
            }

            Category = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var childCategories = await _context.Categories
                .Where(cat => cat.ParentCategoryId == Category.Id)
                .ToListAsync();
            foreach (var childCat in childCategories) {
                childCat.CategoryType = Category.CategoryType;
                _context.Attach(childCat).State = EntityState.Modified;
            }

            if (Category.ParentCategoryId.HasValue) {
                var parentCategory = await _context.Categories.FindAsync(Category.ParentCategoryId);
                Category.ParentCategoryName = parentCategory.Name;
            }

            _context.Attach(Category).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!CategoryExists(Category.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        public IActionResult OnGetParentCategoryAutoComplete(string text) {
            text = text.Trim();
            var categories = _context.Categories
                .Include(cat => !cat.IsChildCategory)
                .AsNoTracking()
                .AsEnumerable()
                .Where(cat => cat.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(cat => new {
                    label = cat.Name,
                    val = cat.Id
                }).ToList();
            return new JsonResult(categories);
        }

        private bool CategoryExists(Guid id) {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
