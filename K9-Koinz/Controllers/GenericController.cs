using K9_Koinz.Data;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Controllers {
    public abstract class GenericController : Controller {
        protected readonly IRepositoryWrapper _data;

        protected GenericController(IRepositoryWrapper data) {
            _data = data;
        }
    }
}
