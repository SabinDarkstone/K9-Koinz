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
                options.EnableSensitiveDataLogging(true);
            });

            builder.Services.AddMvc();

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddLogging(options => {
                options.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            });

            // Add K9 Koinz Services
            builder.Services.AddMyServices();

            // Add K9Koinz Scheduled Jobs
            builder.Services.AddScheduledJobs();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseHttpsRedirection();
                app.UseHsts();
            } else {
                app.UseMigrationsEndPoint();
            }

            app.UseDeveloperExceptionPage();

            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            using (var scope = app.Services.CreateScope()) {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<KoinzContext>();
                context.Database.Migrate();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
