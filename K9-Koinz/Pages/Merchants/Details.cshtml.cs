using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data;

namespace K9_Koinz.Pages.Merchants {
    public class DetailsModel : AbstractDetailsModel<Merchant> {
        public DetailsModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public List<Transaction> Transactions { get; set; }

        protected override async Task<Merchant> QueryRecordAsync(Guid id) {
            var merchant = await _data.MerchantRepository.GetDetailsAsync(id);
            Transactions = merchant.Transactions.ToList();

            return merchant;
        }
    }
}
