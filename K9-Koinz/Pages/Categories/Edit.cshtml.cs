using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Categories {
    public class EditModel : AbstractEditModel<Category> {
        public EditModel(KoinzContext context, IAccountService accountService,
            IAutocompleteService autocompleteService, ITagService tagService)
                : base(context, accountService, autocompleteService, tagService) { }

        protected override async Task<Category> QueryRecordAsync(Guid id) {
            return await _context.Categories
                .Include(cat => cat.ChildCategories)
                .Include(cat => cat.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        protected override async Task BeforeSaveActionsAsync() {
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
