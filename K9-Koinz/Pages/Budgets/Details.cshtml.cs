using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Budgets {
    public class DetailsModel : AbstractDetailsModel<Budget> {
        public DetailsModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public IEnumerable<BudgetLine> BudgetLines { get; set; }

        protected override async Task AdditionalActionsAsync() {
            BudgetLines = await _data.BudgetLines.GetByBudget(Record.Id);

            if (Record.DoNotUseCategories) {
                Record.BudgetedAmount = BudgetLines.First().BudgetedAmount;
            }
        }
    }
}
