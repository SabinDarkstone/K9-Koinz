using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.SavingsGoals {
    public class DeleteModel : AbstractDeleteModel<SavingsGoal> {
        public DeleteModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }
    }
}
