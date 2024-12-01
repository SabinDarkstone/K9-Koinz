using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Budgets {
    public class SetBudgetNameFields : IHandler<Budget> {
        private readonly KoinzContext _context;
        public SetBudgetNameFields(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Budget> oldList, List<Budget> newList) {
            var tags = _context.Tags
                .Where(tag => newList.Select(budget => budget.BudgetTagId).Contains(tag.Id))
                .ToList();

            var tagDict = tags.ToDictionary(tag => tag.Id, tag => tag);

            foreach (var budget in newList) {
                if (budget.BudgetTagId.HasValue && budget.BudgetTagId.Value != Guid.Empty) {
                    budget.BudgetTagName = tagDict[budget.BudgetTagId.Value].Name;
                }
            }
        }
    }
}
