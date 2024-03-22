using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transactions {
    public class DetailsModel : AbstractDetailsModel<Transaction> {
        public DetailsModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Transaction> QueryRecordAsync(Guid id) {
            return await _data.TransactionRepository.GetDetailsAsync(id);
        }
    }
}
