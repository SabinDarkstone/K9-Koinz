using K9_Koinz.Data;
using K9_Koinz.Models;

namespace K9_Koinz.Triggers.Handlers.Bills {
    public class SetBillDefaultValues : IHandler<Bill> {
        private readonly KoinzContext _context;

        public SetBillDefaultValues(KoinzContext context) {
            _context = context;
        }

        public void Execute(List<Bill> oldList, List<Bill> newList) {
            foreach (var bill in newList) {
                if (bill.SavingsGoalId == Guid.Empty) {
                    bill.SavingsGoalId = null;
                }

                if (bill.RepeatConfig == null) {
                    throw new Exception("RepeatConfig must be included in query when inserting a new bill");
                }

                bill.IsActive = true;
            }
        }
    }
}
