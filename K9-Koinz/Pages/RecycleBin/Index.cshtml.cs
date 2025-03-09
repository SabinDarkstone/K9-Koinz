using K9_Koinz.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;

namespace K9_Koinz.Pages.RecycleBin {
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;

        public Dictionary<string, List<object>> DeletedRecords { get; set; } = new();

        public IndexModel(KoinzContext context) {
            this._context = context;
        }

        public async Task<IActionResult> OnGetAsync() {
            var dbSets = _context.GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType &&
                       p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToList();

            foreach (var dbSetProperty in dbSets) {
                var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                var dbSet = dbSetProperty.GetValue(_context);

                var isDeletedProperty = entityType.GetProperty("IsDeleted");
                if (isDeletedProperty != null) {
                    var query = typeof(Queryable)
                        .GetMethods()
                        .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(entityType);

                    var param = Expression.Parameter(entityType, "e");
                    var condition = Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(param, isDeletedProperty),
                            Expression.Constant(true)
                        ),
                        param
                    );

                    var dbSetQuery = typeof(EntityFrameworkQueryableExtensions)
                        .GetMethod(nameof(EntityFrameworkQueryableExtensions.IgnoreQueryFilters))
                        .MakeGenericMethod(entityType)
                        .Invoke(null, new[] { dbSet });

                    var filteredQuery = query.Invoke(null, new[] { dbSetQuery, condition });
                    
                    var toListMethod = typeof(Enumerable)
                        .GetMethods()
                        .First(m => m.Name == "ToList" && m.IsGenericMethod)
                        .MakeGenericMethod(entityType);

                    var resultList = toListMethod.Invoke(null, new[] { filteredQuery });

                    if (resultList is IList list && list.Count > 0) {
                        DeletedRecords.Add(dbSetProperty.Name, list.Cast<object>().ToList());
                    }
                }
            }

            return Page();
        }
    }
}
