using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Tags {
    public class IndexModel : AbstractIndexModel<Tag> {
        public IndexModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public async Task OnGetAsync() {
            RecordList = await _data.Tags.GetAll();
        }
    }
}
