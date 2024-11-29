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
        TriggerStatus OnBeforeInsert(List<TEntity> newList);
        TriggerStatus OnAfterInsert(List<TEntity> newList);
        TriggerStatus OnBeforeUpdate(List<TEntity> oldList, List<TEntity> newList);
        TriggerStatus OnAfterUpdate(List<TEntity> oldList, List<TEntity> newList);
        TriggerStatus OnBeforeDelete(List<TEntity> oldList);
        TriggerStatus OnAfterDelete(List<TEntity> newList);
    }
}
