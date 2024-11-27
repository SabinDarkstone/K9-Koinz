using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Utils;
using Serilog;

namespace K9_Koinz {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services, builder.Configuration);
            var app = builder.Build();
            ConfigureMiddleware(app);
            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<KoinzContext>(options => {
                options.UseSqlite("Data Source=K9-Koinz.db");
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });

            services.AddMvc();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Console()
                .Filter.ByExcluding(line => line.RenderMessage().Contains("Executed DbCommand"))
                .WriteTo.File("logs/k9-koinz.log", rollingInterval: RollingInterval.Infinite)
                .CreateLogger();

            services.AddRazorPages();
            services.AddControllers();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddLogging(options => {
                options.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                options.AddSerilog(dispose: true);
            });

            services.AddMyServices();
            services.AddScheduledJobs();
        }

        private static void ConfigureMiddleware(WebApplication app) {
            if (!app.Environment.IsDevelopment()) {
                app.UseHttpsRedirection();
            } else {
                app.UseMigrationsEndPoint();
            }

            app.UseHsts();

            // Create robots.txt
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

            using (var scope = app.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<KoinzContext>();
                context.Database.Migrate();
            }

            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            app.UseRouting();
            app.MapRazorPages();
            app.MapControllers();
            app.MapControllerRoute("default", "api/{controller}/{action}/{id?}");
            app.UseAuthorization();
        }

        //public static void Main(string[] args) {
        //    var builder = WebApplication.CreateBuilder(args);
        //    builder.Services.AddDbContext<KoinzContext>(options => {
        //        options.UseSqlite("Data Source=K9-Koinz.db");
        //        options.EnableDetailedErrors();
        //        options.EnableSensitiveDataLogging();
        //    });

        //    builder.Services.AddMvc();
        //    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        //    Log.Logger = new LoggerConfiguration()
        //        .MinimumLevel.Warning()
        //        .WriteTo.Console()
        //        .Filter.ByExcluding(line => line.RenderMessage().Contains("Executed DbCommand"))
        //        .WriteTo.File("logs/k9-koinz.log", rollingInterval: RollingInterval.Infinite)
        //        .CreateLogger();

        //    // Add services to the container.
        //    builder.Services.AddRazorPages();
        //    builder.Services.AddControllers();
        //    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        //    builder.Services.AddLogging(options => {
        //        options.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        //        options.AddSerilog(dispose: true);
        //    });

        //    // Add K9 Koinz Services
        //    builder.Services.AddMyServices();

        //    // Add K9Koinz Scheduled Jobs
        //    builder.Services.AddScheduledJobs();

        //    var app = builder.Build();

        //    // Configure the HTTP request pipeline.
        //    if (!app.Environment.IsDevelopment()) {
        //        app.UseHttpsRedirection();
        //    } else {
        //        app.UseMigrationsEndPoint();
        //    }

        //    app.UseHsts();

        //    app.Use(async (context, next) => {
        //        if (context.Request.Path.StartsWithSegments("/robots.txt")) {
        //            var robotsTxtPath = Path.Combine(app.Environment.ContentRootPath, "robots.txt");
        //            string output = "User-agent: * \nDisallow: /";
        //            if (File.Exists(robotsTxtPath)) {
        //                output = await File.ReadAllTextAsync(robotsTxtPath);
        //            }
        //            context.Response.ContentType = "text/plain";
        //            await context.Response.WriteAsync(output);
        //        } else await next();
        //    });

        //    using (var scope = app.Services.CreateScope()) {
        //        var services = scope.ServiceProvider;

        //        var context = services.GetRequiredService<KoinzContext>();
        //        context.Database.Migrate();
        //    }

        //    app.UseStaticFiles();

        //    app.UseDeveloperExceptionPage();
        //    app.UseStatusCodePagesWithReExecute("/Errors/{0}");

        //    app.UseRouting();

        //    app.MapRazorPages();
        //    app.MapControllers();
        //    app.MapControllerRoute("default", "api/{controller}/{action}/{id?}");

        //    app.UseAuthorization();


        //    app.Run();
        //}
    }
}
