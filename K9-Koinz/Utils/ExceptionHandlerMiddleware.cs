using K9_Koinz.Data;
using K9_Koinz.Models;
using Newtonsoft.Json;

namespace K9_Koinz.Utils {
    public class ExceptionHandlerMiddleware {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlerMiddleware(RequestDelegate next, IWebHostEnvironment env) {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context, IServiceScopeFactory scopeFactory) {
            try {
                await _next(context);
            } catch (Exception ex) {
                await HandleExceptionAsync(context, ex, scopeFactory);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception exception, IServiceScopeFactory scopeFactory) {
            if (context.Response.StatusCode == 404) {
                return;
            }

            if (_env.IsDevelopment() && false) {
                // Show developer error page
                context.Response.Clear();
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";

                var developerErrorMessage = $@"
                    Exception: {exception.GetType().Name}
                    Message: {exception.Message}
                    StackTrace:
                    {exception.StackTrace}
                ";
                await context.Response.WriteAsync(developerErrorMessage);
                return;
            }

            List<ErrorLog> logs = new List<ErrorLog>();
            Exception parentException = null;
            ErrorLog parentError = null;
            while (exception != null) {
                ErrorLog error = new ErrorLog {
                    ClassName = exception.GetType().Name,
                    Message = exception.Message,
                    StackTraceString = exception.StackTrace,
                    ExceptionString = JsonConvert.SerializeObject(exception),
                    CurrentRoute = context.Request.Path
                };

                if (parentException != null) {
                    parentError.InnerException = error;
                }

                parentException = exception;
                parentError = error;
                exception = exception.InnerException;

                logs.Add(error);
            }

            using (var scope = scopeFactory.CreateScope()) {
                var db = scope.ServiceProvider.GetRequiredService<KoinzContext>();

                db.Errors.AddRange(logs);
                await db.SaveChangesAsync();
            }

            context.Response.Redirect("/Errors/500");
        }
    }
}
