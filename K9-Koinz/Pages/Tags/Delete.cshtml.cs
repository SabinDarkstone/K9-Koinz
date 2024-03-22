using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Tags {
    public class DeleteModel : AbstractDeleteModel<Tag> {
        public DeleteModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }
    }
}
