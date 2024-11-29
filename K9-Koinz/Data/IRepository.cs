using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;
using K9_Koinz.Triggers;

namespace K9_Koinz.Data {
    public interface IRepository<TEntity> where TEntity : BaseEntity {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(Guid id, bool doTracking = true);
        Task<List<TEntity>> GetByIdsAsync(List<Guid> idList, bool doTracking = true);
        Task<DbSaveResult> AddAsync(TEntity entity);
        Task<DbSaveResult> AddManyAsync(IEnumerable<TEntity> entities);
        Task<DbSaveResult> UpdateAsync(TEntity entity);
        Task<DbSaveResult> UpdateManyAsync(IList<TEntity> entities);
        Task<DbSaveResult> DeleteAsync(Guid id);
        Task<DbSaveResult> DeleteManyAsync(IList<Guid> idList);


        TriggerActionResult BeforeSave(TriggerType triggerType, IEnumerable<TEntity> oldList, IEnumerable<TEntity> newList);
        TriggerActionResult AfterSave(TriggerType triggerType, IEnumerable<TEntity> oldList, IEnumerable<TEntity> newList);
    }
}
