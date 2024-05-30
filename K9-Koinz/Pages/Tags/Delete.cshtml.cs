using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Authorization;

namespace K9_Koinz.Pages.Tags {
    [Authorize]
    public class DeleteModel : AbstractDeleteModel<Tag> {
        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }
    }
}
