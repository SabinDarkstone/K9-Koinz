using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.SavingsGoals {
    public class DetailsModel : AbstractDetailsModel<SavingsGoal> {
        public DetailsModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<SavingsGoal> QueryRecordAsync(Guid id) {
            return await _data.SavingsGoalRepository.GetDetailsAsync(id);
        }
    }
}
