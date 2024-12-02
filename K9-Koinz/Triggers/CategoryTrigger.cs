using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Triggers.Handlers.Categories;

namespace K9_Koinz.Triggers {
    public class CategoryTrigger : GenericTrigger<Category> {
        public CategoryTrigger(KoinzContext context) : base(context) { }

        public override TriggerStatus OnBeforeInsert(List<Category> newList) {
            new SetMyParentCategoryName(context).Execute(null, newList);

            return TriggerStatus.SUCCESS;
        }

        public override TriggerStatus OnAfterDelete(List<Category> newList) {
            new DeleteChildCategories(context).Execute(null, newList);

            return TriggerStatus.SUCCESS;
        }

        public override TriggerStatus OnBeforeUpdate(List<Category> oldList, List<Category> newList) {
            new SetMyParentCategoryName(context).Execute(oldList, newList);

            return TriggerStatus.SUCCESS;
        }

        public override TriggerStatus OnAfterUpdate(List<Category> oldList, List<Category> newList) {
            new UpdateCategoryNames(context).Execute(oldList, newList);
            new UpdateChildCategoryType(context).Execute(oldList, newList);
            new UpdateParentCategoryName(context).Execute(null, newList);

            return TriggerStatus.SUCCESS;
        }
    }
}
