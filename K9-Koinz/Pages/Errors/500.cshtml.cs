using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace K9_Koinz.Pages.Errors {
    public class _500Model : PageModel {

        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string Path { get; set; }

        public void OnGet() {
            var exceptionPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            Path = exceptionPathFeature.Path;
            ErrorMessage = exceptionPathFeature.Error.Message;
            StackTrace = exceptionPathFeature.Error.StackTrace;
        }
    }
}
