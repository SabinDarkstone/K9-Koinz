using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Bills {
    public class DetailsModel : DetailsPageModel<Bill> {
        public DetailsModel(BillRepository repository) : base(repository) { }

        protected override async Task<Bill> QueryRecord(Guid id) {
            return await (_repository as BillRepository).GetBillWithDetails(id);
        }

        protected override void AfterQueryActions() {
            Record.Transactions = (_repository as BillRepository).GetTransactionsForBill(Record.Id);
        }
    }
}
