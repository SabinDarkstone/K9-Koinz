using K9_Koinz.Services;
using K9_Koinz.Services.BackgroundWorkers;

namespace K9_Koinz.Utils {
    public static class ServiceManager {
        public static void AddMyServices(this IServiceCollection services) {
            services.AddScoped<ISpendingGraphService, SpendingGraphService>();
            services.AddScoped<IDbCleanupService, DbCleanupService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAutocompleteService, AutocompleteService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IBudgetService, BudgetService>();
        }

        public static void AddScheduledJobs(this IServiceCollection services) {
            services.AddHostedService<DailyBillToTransactionJob>();
            services.AddHostedService<RecurringTransferJob>();
        }
    }
}
