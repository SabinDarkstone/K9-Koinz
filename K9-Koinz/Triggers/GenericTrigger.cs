using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using K9_Koinz.Triggers.Handlers;

namespace K9_Koinz.Triggers {
    public class GenericTrigger<TEntity> where TEntity : BaseEntity {
        protected Dictionary<string, AbstractTriggerHandler<TEntity>> handlers;

        protected readonly KoinzContext context;
        protected GenericTrigger(KoinzContext context) {
            this.context = context;
        }
    }
}
