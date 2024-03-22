using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transfers.Recurring {
    public class DetailsModel : AbstractDetailsModel<Transfer> {
        public DetailsModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Transfer> QueryRecordAsync(Guid id) {
            return await _data.Transfers.GetDetails(id);
        }
    }
}
