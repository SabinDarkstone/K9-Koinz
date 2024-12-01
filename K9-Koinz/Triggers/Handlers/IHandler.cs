using K9_Koinz.Models.Meta;

namespace K9_Koinz.Triggers.Handlers {
    public interface IHandler<TEntity> where TEntity : BaseEntity {
        void Execute(List<TEntity> oldList, List<TEntity> newList);
    }
}
