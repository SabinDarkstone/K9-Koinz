using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Budgets {
    public class DeleteModel : DeletePageModel<Budget> {
        public List<BudgetLine> BudgetLines { get; set; }

        public DeleteModel(BudgetRepository repository) : base(repository) { }

        protected override void AfterQueryActions() {
            BudgetLines = (_repository as BudgetRepository).GetBudgetLinesByBudgetId(Record.Id);
        }
    }
}
