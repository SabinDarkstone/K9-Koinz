using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers {
    public class CategoryTrigger : GenericTrigger<Category> {
        public CategoryTrigger(KoinzContext context) : base(context) { }
    }
}
