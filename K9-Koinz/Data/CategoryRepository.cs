using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace K9_Koinz.Data {
    public class CategoryRepository : GenericRepository<Category> {
        public CategoryRepository(KoinzContext context) : base(context) { }

        public async Task<Category> GetCategoryDetails(Guid id) {
            return await DbSet
                .Include(cat => cat.ParentCategory)
                .Include(cat => cat.ChildCategories
                    .OrderBy(cCat => cCat.Name))
                .AsNoTracking()
                .SingleOrDefaultAsync(cat => cat.Id == id);
        }

        public async Task<IEnumerable<Category>> GetAll() {
            return await DbSet
                .AsNoTracking()
                .Include(cat => cat.ChildCategories)
                    .ThenInclude(cCat => cCat.Transactions)
                .Include(cat => cat.Transactions)
                .OrderBy(cat => cat.CategoryType)
                    .ThenBy(cat => cat.Name)
                .ToListAsync();
        }

        public async Task<SelectList> GetDropdown() {
            var categories = await DbSet.OrderBy(cat => cat.Name).ToListAsync();
            return new SelectList(categories, nameof(Category.Id), nameof(Category.Name));
        }

        public async Task<IEnumerable<Category>> GetChildrenAsync(Guid parentCategoryId) {
            return await DbSet
                .Where(cat => cat.ParentCategoryId == parentCategoryId)
                .OrderBy(cat => cat.Name)
                .ToListAsync();
        }

        public Category GetByName(string name) {
            return DbSet
                .Where(cat => cat.Name == name)
                .SingleOrDefault();
        }

        public async Task<IEnumerable<AutocompleteResult>> GetForAutocomplete(string searchText) {
            return (await DbSet
                .Include(cat => cat.ParentCategory)
                .AsNoTracking()
                .ToListAsync())
                .Where(cat => cat.FullyQualifiedName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                .Select(cat => new AutocompleteResult {
                    Label = cat.ParentCategoryId != null ? cat.ParentCategory.Name + ": " + cat.Name : cat.Name,
                    Id = cat.Id
                }).ToList();
        }

        public async Task<IEnumerable<AutocompleteResult>> GetParentCategoryAutocomplete(string searchText) {
            return (await DbSet
                .Include(cat => !cat.IsChildCategory)
                .AsNoTracking()
                .ToListAsync())
                .Where(cat => cat.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                .Select(cat => new AutocompleteResult {
                    Label = cat.Name,
                    Id = cat.Id
                }).ToList();
        }
    }
}