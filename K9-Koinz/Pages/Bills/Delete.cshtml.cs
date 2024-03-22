using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Bills {
    public class DeleteModel : AbstractDeleteModel<Bill> {
        public DeleteModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Bill> QueryRecordAsync(Guid id) {
            return await _data.BillRepository.GetDetails(id);
        }
    }
}
