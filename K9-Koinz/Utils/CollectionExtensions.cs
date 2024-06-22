using K9_Koinz.Models.Meta;
using System.Reflection;

namespace K9_Koinz.Utils {
    public static class CollectionExtensions {

        public static List<Guid> GetIds<TEntity>(this List<TEntity> records) where TEntity : BaseEntity {
            return records.Select(x => x.Id).ToList();
        }
    }
}
