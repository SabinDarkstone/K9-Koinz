using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Bills {
    public class DeleteModel : DeletePageModel<Bill> {
        public DeleteModel(BillRepository repository) : base(repository) { }

        protected override async Task<Bill> QueryRecord(Guid id) {
            return await (_repository as BillRepository).GetBillWithDetails(id);
        }
    }
}
