using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Tags {
    public class DeleteModel : DeletePageModel<Tag> {
        public DeleteModel(TagRepository repository) : base(repository) { }
    }
}
