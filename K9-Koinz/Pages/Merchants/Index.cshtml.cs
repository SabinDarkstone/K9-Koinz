using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Merchants {
    public class IndexModel : AbstractIndexModel<Merchant> {
        public IndexModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public async Task OnGetAsync() {
            RecordList = await _data.MerchantRepository.GetAll();
        }
    }
}
