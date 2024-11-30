using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Merchants {
    public class DeleteModel : DeletePageModel<Merchant> {
        public DeleteModel(MerchantRepository repository) : base(repository) { }

        protected override Task<Merchant> QueryRecord(Guid id) {
            return (_repository as MerchantRepository).GetMerchantWithTransactionsById(id);
        }
    }
}
