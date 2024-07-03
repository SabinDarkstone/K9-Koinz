using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace K9_Koinz.Triggers {
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
        abstract void SetState(ModelStateDictionary state);
        abstract void OnBeforeInsert(List<TEntity> newList);
        abstract void OnAfterInsert(List<TEntity> newList);
        abstract void OnBeforeUpdate(List<TEntity> oldList, List<TEntity> newList);
        abstract void OnAfterUpdate(List<TEntity> oldList, List<TEntity> newList);
        abstract void OnBeforeDelete(List<TEntity> oldList);
        abstract void OnAfterDelete(List<TEntity> newList);
    }
}
