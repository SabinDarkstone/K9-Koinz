using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.ViewComponents {
    [ViewComponent(Name = "DateFields")]
    public class DateFieldsController : ViewComponent {

        public BaseEntity Record { get; set; }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IViewComponentResult> InvokeAsync(BaseEntity record) {
            Record = record;
            return View(this);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
