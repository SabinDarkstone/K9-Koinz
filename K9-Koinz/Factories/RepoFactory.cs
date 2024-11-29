using K9_Koinz.Data;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Factories {

    public interface IRepoFactory {
        IRepository<TEntity> CreateRepository<TEntity>() where TEntity : BaseEntity;
        TRepository CreateSpecializedRepository<TRepository>() where TRepository : class;
    }

    public class RepoFactory : IRepoFactory {
        private readonly IServiceProvider _serviceProvider;

        public RepoFactory(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public IRepository<TEntity> CreateRepository<TEntity>() where TEntity : BaseEntity {
            return _serviceProvider.GetRequiredService<IRepository<TEntity>>();
        }

        public TRepository CreateSpecializedRepository<TRepository>() where TRepository : class {
            return _serviceProvider.GetRequiredService<TRepository>();
        }
    }
}
