using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using Microsoft.AspNetCore.HttpOverrides;
using K9_Koinz.Services;
using K9_Koinz.Services.BackgroundWorkers;
namespace K9_Koinz {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<KoinzContext>(options => {
                options.UseSqlite("Data Source=K9-Koinz.db");
                options.EnableSensitiveDataLogging(true);
            });

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddLogging(options => {
                options.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            });

            builder.Services.AddScoped<ISpendingGraphService, SpendingGraphService>();
            builder.Services.AddScoped<IDbCleanupService, DbCleanupService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IAutocompleteService, AutocompleteService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IBudgetService, BudgetService>();

            builder.Services.AddHostedService<ScheduledTransactionCreation>();

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
                //DbInitializer.Initialize(context);
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
