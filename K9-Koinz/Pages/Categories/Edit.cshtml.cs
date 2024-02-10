using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Categories {
    public class EditModel : AbstractEditModel<Category> {
        public EditModel(KoinzContext context, IAccountService accountService, IAutocompleteService autocompleteService, ITagService tagService)
            : base(context, accountService, autocompleteService, tagService) { }

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

            Record = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var childCategories = await _context.Categories
                .Where(cat => cat.ParentCategoryId == Record.Id)
                .ToListAsync();
            foreach (var childCat in childCategories) {
                childCat.CategoryType = Record.CategoryType;
                _context.Attach(childCat).State = EntityState.Modified;
            }

            if (Record.ParentCategoryId.HasValue) {
                var parentCategory = await _context.Categories.FindAsync(Record.ParentCategoryId);
                Record.ParentCategoryName = parentCategory.Name;
            }

            _context.Attach(Record).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!RecordExists(Record.Id)) {
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
    }
}
