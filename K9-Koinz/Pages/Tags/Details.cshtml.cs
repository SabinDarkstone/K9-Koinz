using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Tags {
    public class DetailsModel : AbstractDetailsModel<Tag> {
        public DetailsModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<Tag> QueryRecordAsync(Guid id) {
            return await _data.Tags.GetDetails(id);
        }
    }
}
