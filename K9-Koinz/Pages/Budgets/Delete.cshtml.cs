using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Budgets {
    public class DeleteModel : AbstractDeleteModel<Budget> {
        public IEnumerable<BudgetLine> BudgetLines { get; set; }
        public DeleteModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task AfterQueryActionAsync() {
            BudgetLines = await _data.BudgetLineRepository.GetByBudget(Record.Id);

            if (Record.DoNotUseCategories) {
                Record.BudgetedAmount = BudgetLines.First().BudgetedAmount;
            }
        }
    }
}
