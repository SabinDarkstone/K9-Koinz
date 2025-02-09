using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;
using K9_Koinz.Triggers;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data.Repositories.Meta {
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

        public async Task<List<TEntity>> GetByIdsAsync(List<Guid> idList, bool doTracking = true) {
            if (doTracking) {
                return await _dbSet.Where(e => idList.Contains(e.Id))
                    .OrderBy(e => e.Id)
                    .ToListAsync();
            }

            return await _dbSet.AsNoTracking()
                .Where(e => idList.Contains(e.Id))
                .OrderBy(e => e.Id)
                .ToListAsync();
        }

        // TODO: Add error handling
        public async Task<DbSaveResult> AddAsync(TEntity entity) {
            return await AddManyAsync([entity]);
        }

        public async Task<DbSaveResult> AddManyAsync(IEnumerable<TEntity> entities) {
            var beforeResult = BeforeSave(TriggerType.INSERT, null, entities);
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            var afterResult = AfterSave(TriggerType.INSERT, null, entities);

            DbSaveResult.BeforeStatus = beforeResult.Status;
            DbSaveResult.AfterStatus = afterResult.Status;
            DbSaveResult.ErrorMessage = beforeResult.ErrorMessage + " " + afterResult.ErrorMessage;
            DbSaveResult.Ids.AddRange(entities.Select(e => e.Id));

            return DbSaveResult;
        }

        public async Task<DbSaveResult> UpdateAsync(TEntity entity) {
            return await UpdateManyAsync([entity]);
        }

        // TODO: Add error handling
        public async Task<DbSaveResult> UpdateManyAsync(IList<TEntity> entities) {
            List<TEntity> oldEntities = await GetByIdsAsync(entities.Select(e => e.Id).ToList(), false);
            entities = entities.OrderBy(e => e.Id).ToList();
            var beforeResult = BeforeSave(TriggerType.UPDATE, oldEntities, entities);
            _dbSet.UpdateRange(entities);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            var afterResult = AfterSave(TriggerType.UPDATE, oldEntities, entities);

            DbSaveResult.BeforeStatus = beforeResult.Status;
            DbSaveResult.AfterStatus = afterResult.Status;
            DbSaveResult.ErrorMessage = beforeResult.ErrorMessage + " " + afterResult.ErrorMessage;
            DbSaveResult.Ids.AddRange(entities.Select(e => e.Id));

            return DbSaveResult;
        }

        public async Task<DbSaveResult> DeleteAsync(Guid id) {
            return await DeleteManyAsync([id]);
        }

        public async Task<DbSaveResult> DeleteManyAsync(IList<Guid> idList) {
            var entities = await GetByIdsAsync(idList.ToList());
            if (entities != null) {
                var beforeResult = BeforeSave(TriggerType.DELETE, entities, null);
                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
                var afterResult = AfterSave(TriggerType.DELETE, entities, null);

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
