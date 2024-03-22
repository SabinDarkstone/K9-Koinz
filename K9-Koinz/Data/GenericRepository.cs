using K9_Koinz.Models.Meta;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity {
        void Add(IEnumerable<TEntity> entities);
        void Add(TEntity entity);
        Task<bool> DoesExistAsync(Guid id);
        Task<TEntity> GetByIdAsync(Guid? id);
        void Remove(Guid id);
        void Remove(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void Update(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
    }

    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity {
        private readonly KoinzContext _context;

        public GenericRepository(KoinzContext context) {
            _context = context;
        }

        public DbSet<TEntity> DbSet => _context.Set<TEntity>();

        public virtual async Task<TEntity> GetByIdAsync(Guid? id) {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<bool> DoesExistAsync(Guid id) {
            return await _context.Set<TEntity>().AnyAsync(x => x.Id == id);
        }

        public virtual void Add(TEntity entity) {
            _context.Set<TEntity>().Add(entity);
        }

        public virtual void Add(IEnumerable<TEntity> entities) {
            _context.Set<TEntity>().AddRange(entities);
        }

        public virtual void Update(TEntity entity) {
            _context.Set<TEntity>().Update(entity);
        }

        public virtual void Update(IEnumerable<TEntity> entities) {
            _context.Set<TEntity>().UpdateRange(entities);
        }

        public virtual void Remove(Guid id) {
            Remove(_context.Set<TEntity>().Find(id));
        }

        public virtual void Remove(TEntity entity) {
            _context.Set<TEntity>().Remove(entity);
        }

        public virtual void Remove(IEnumerable<TEntity> entities) {
            _context.Set<TEntity>().RemoveRange(entities);
        }
    }
}
