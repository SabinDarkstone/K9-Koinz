using K9_Koinz.Data.Repositories.Meta;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;
using K9_Koinz.Triggers;

namespace K9_Koinz.Data.Repositories {
    public class TriggeredRepository<TEntity> : Repository<TEntity> where TEntity : BaseEntity {
        private readonly ITrigger<TEntity> _trigger;

        public TriggeredRepository(KoinzContext context, ITrigger<TEntity> trigger) : base(context) {
            _trigger = trigger;
        }

        public override TriggerActionResult BeforeSave(TriggerType triggerType, IEnumerable<TEntity> oldList, IEnumerable<TEntity> newList) {
            var status = TriggerStatus.SUCCESS;

            switch (triggerType) {
                case TriggerType.INSERT:
                    status = _trigger?.OnBeforeInsert(newList.ToList()) ?? TriggerStatus.NO_TRIGGER;
                    break;
                case TriggerType.UPDATE:
                    status = _trigger?.OnBeforeUpdate(oldList.ToList(), newList.ToList()) ?? TriggerStatus.NO_TRIGGER;
                    break;
                case TriggerType.DELETE:
                    status = _trigger?.OnBeforeDelete(oldList.ToList()) ?? TriggerStatus.NO_TRIGGER;
                    break;
            }

            return new TriggerActionResult {
                Status = status,
                ErrorMessage = string.Empty,
                Exception = null
            };
        }

        public override TriggerActionResult AfterSave(TriggerType triggerType, IEnumerable<TEntity> oldList, IEnumerable<TEntity> newList) {
            var status = TriggerStatus.SUCCESS;

            switch (triggerType) {
                case TriggerType.INSERT:
                    status = _trigger?.OnAfterInsert(newList.ToList()) ?? TriggerStatus.NO_TRIGGER;
                    break;
                case TriggerType.UPDATE:
                    status = _trigger?.OnAfterUpdate(oldList.ToList(), newList.ToList()) ?? TriggerStatus.NO_TRIGGER;
                    break;
                case TriggerType.DELETE:
                    status = _trigger?.OnAfterDelete(oldList.ToList()) ?? TriggerStatus.NO_TRIGGER;
                    break;
            }

            if (status != TriggerStatus.ERROR) {
                _context.SaveChanges();
            }

            return new TriggerActionResult {
                Status = status,
                ErrorMessage = string.Empty,
                Exception = null
            };
        }
    }
}
