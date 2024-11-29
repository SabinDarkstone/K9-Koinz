using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;
using K9_Koinz.Triggers;

namespace K9_Koinz.Data {
    public interface IRepository<TEntity> where TEntity : BaseEntity {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(Guid id, bool doTracking = true);
        Task<DbSaveResult> AddAsync(TEntity entity);
        Task<DbSaveResult> UpdateAsync(TEntity entity);
        Task<DbSaveResult> DeleteAsync(Guid id);


        TriggerActionResult BeforeSave(TriggerType triggerType, IEnumerable<TEntity> oldList, IEnumerable<TEntity> newList);
        TriggerActionResult AfterSave(TriggerType triggerType, IEnumerable<TEntity> oldList, IEnumerable<TEntity> newList);
    }
}
