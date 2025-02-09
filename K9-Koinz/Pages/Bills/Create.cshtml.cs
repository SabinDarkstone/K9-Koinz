using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Bills {
    public class CreateModel : CreatePageModel<Bill> {
        public CreateModel(BillRepository repository) : base(repository) { }
    }
}
