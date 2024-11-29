using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Triggers {

    public enum TriggerType {
        INSERT,
        UPDATE,
        DELETE,
        UNDELETE
    }

    public enum Status {
        SUCCESS,
        ERROR,
        NULL
    }

    public static class StatusExtensions {
        public static Status TryGetValue2<TKey, TValue>(this Dictionary<TKey, TValue> entityDict, TKey key, out TValue value) {
            return entityDict.TryGetValue(key, out value) ? Status.SUCCESS : Status.ERROR;
        }
    }

    public interface ITrigger<TEntity> where TEntity : BaseEntity {
        abstract TriggerStatus OnBeforeInsert(List<TEntity> newList);
        abstract TriggerStatus OnAfterInsert(List<TEntity> newList);
        abstract TriggerStatus OnBeforeUpdate(List<TEntity> oldList, List<TEntity> newList);
        abstract TriggerStatus OnAfterUpdate(List<TEntity> oldList, List<TEntity> newList);
        abstract TriggerStatus OnBeforeDelete(List<TEntity> oldList);
        abstract TriggerStatus OnAfterDelete(List<TEntity> newList);
    }
}
