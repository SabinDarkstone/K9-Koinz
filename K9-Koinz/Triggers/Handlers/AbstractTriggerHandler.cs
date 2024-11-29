using K9_Koinz.Data;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Triggers.Handlers {
    public abstract class AbstractTriggerHandler<TEntity> where TEntity : BaseEntity {
        protected readonly KoinzContext context;

        protected AbstractTriggerHandler(KoinzContext context) {
            this.context = context;
        }
    }
}
