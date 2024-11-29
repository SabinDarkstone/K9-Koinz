using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;
using K9_Koinz.Triggers;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity {
        protected readonly KoinzContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        private DbSaveResult DbSaveResult { get; set; }

        public Repository(KoinzContext context) {
            _context = context;
            _dbSet = context.Set<TEntity>();
            DbSaveResult = new DbSaveResult();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id, bool doTracking = true) {
            if (doTracking) {
                return await _dbSet.FindAsync(id);
            }

            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        // TODO: Add error handling
        public async Task<DbSaveResult> AddAsync(TEntity entity) {
            var beforeResult = BeforeSave(TriggerType.INSERT, null, [entity]);
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            var afterResult = AfterSave(TriggerType.INSERT, null, [entity]);

            DbSaveResult.BeforeStatus = beforeResult.Status;
            DbSaveResult.AfterStatus = afterResult.Status;
            DbSaveResult.ErrorMessage = beforeResult.ErrorMessage +  " " + afterResult.ErrorMessage;

            return DbSaveResult;
        }

        // TODO: Add error handling
        public async Task<DbSaveResult> UpdateAsync(TEntity entity) {
            TEntity oldEntity = await GetByIdAsync(entity.Id, false);
            var beforeResult = BeforeSave(TriggerType.UPDATE, [oldEntity], [entity]);
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            var afterResult = AfterSave(TriggerType.UPDATE, [oldEntity], [entity]);

            DbSaveResult.BeforeStatus = beforeResult.Status;
            DbSaveResult.AfterStatus = afterResult.Status;
            DbSaveResult.ErrorMessage = beforeResult.ErrorMessage + " " + afterResult.ErrorMessage;

            return DbSaveResult;
        }

        public async Task<DbSaveResult> DeleteAsync(Guid id) {
            var entity = await GetByIdAsync(id);
            if (entity != null) {
                var beforeResult = BeforeSave(TriggerType.DELETE, [entity], null);
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                var afterResult = AfterSave(TriggerType.DELETE, [entity], null);

                DbSaveResult.BeforeStatus = beforeResult.Status;
                DbSaveResult.AfterStatus = afterResult.Status;
            } else {
                DbSaveResult.ErrorMessage = "Entity not found";
                DbSaveResult.Status = SaveStatus.ERROR;
            }

            return DbSaveResult;
        }

        public virtual TriggerActionResult BeforeSave(TriggerType triggerType, IEnumerable<TEntity> oldList, IEnumerable<TEntity> newList) {
            return new TriggerActionResult {
                Status = TriggerStatus.NO_TRIGGER,
                ErrorMessage = string.Empty,
                Exception = null
            };
        }

        public virtual TriggerActionResult AfterSave(TriggerType triggerType, IEnumerable<TEntity> oldList, IEnumerable<TEntity> newList) {
            return new TriggerActionResult {
                Status = TriggerStatus.NO_TRIGGER,
                ErrorMessage = string.Empty,
                Exception = null
            };
        }
    }
}
