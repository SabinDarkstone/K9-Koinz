using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Merchants {
    public class CreateModel : CreatePageModel<Merchant> {
        public CreateModel(MerchantRepository repository) : base(repository) {
        }
    }
}
