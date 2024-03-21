using K9_Koinz.Models;

namespace K9_Koinz.Data {
    public class BudgetLinePeriodRepository : GenericRepository<BudgetLinePeriod> {
        public BudgetLinePeriodRepository(KoinzContext context) : base(context) { }
    }
}