using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using Microsoft.AspNetCore.HttpOverrides;
using K9_Koinz.Utils;
namespace K9_Koinz {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<KoinzContext>(options => {
                options.UseSqlite("Data Source=K9-Koinz.db");
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });

            builder.Services.AddMvc();

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddLogging(options => {
                options.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            });

            if (!builder.Environment.IsDevelopment()) {
                builder.Services.AddHttpsRedirection(options => {
                    options.RedirectStatusCode = 308;
                    options.HttpsPort = 443;
                });
            }

            // Add K9 Koinz Services
            builder.Services.AddMyServices();

            // Add K9Koinz Scheduled Jobs
            builder.Services.AddScheduledJobs();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseHsts();
            } else {
                app.UseMigrationsEndPoint();
            }

            app.UseHttpsRedirection();

            app.Use(async (context, next) => {
                if (context.Request.Path.StartsWithSegments("/robots.txt")) {
                    var robotsTxtPath = Path.Combine(app.Environment.ContentRootPath, "robots.txt");
                    string output = "User-agent: * \nDisallow: /";
                    if (File.Exists(robotsTxtPath)) {
                        output = await File.ReadAllTextAsync(robotsTxtPath);
                    }
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(output);
                } else await next();
            });

            app.UseDeveloperExceptionPage();

            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            using (var scope = app.Services.CreateScope()) {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<KoinzContext>();
                context.Database.Migrate();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
