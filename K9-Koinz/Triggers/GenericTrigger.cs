using K9_Koinz.Data;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;
using K9_Koinz.Triggers.Handlers;

namespace K9_Koinz.Triggers {
    public class GenericTrigger<TEntity> : ITrigger<TEntity> where TEntity : BaseEntity {
        protected Dictionary<string, AbstractTriggerHandler<TEntity>> handlers;

        protected readonly KoinzContext context;
        public GenericTrigger(KoinzContext context) {
            this.context = context;
        }

        public virtual TriggerStatus OnAfterDelete(List<TEntity> newList) {
            return TriggerStatus.NO_TRIGGER;
        }

        public virtual TriggerStatus OnAfterInsert(List<TEntity> newList) {
            return TriggerStatus.NO_TRIGGER;
        }

        public virtual TriggerStatus OnAfterUpdate(List<TEntity> oldList, List<TEntity> newList) {
            return TriggerStatus.NO_TRIGGER;
        }

        public virtual TriggerStatus OnBeforeDelete(List<TEntity> oldList) {
            return TriggerStatus.NO_TRIGGER;
        }

        public virtual TriggerStatus OnBeforeInsert(List<TEntity> newList) {
            return TriggerStatus.NO_TRIGGER;
        }

        public virtual TriggerStatus OnBeforeUpdate(List<TEntity> oldList, List<TEntity> newList) {
            return TriggerStatus.NO_TRIGGER;
        }
    }
}
