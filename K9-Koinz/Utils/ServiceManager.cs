using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services;
using K9_Koinz.Services.BackgroundWorkers;

namespace K9_Koinz.Utils {
    public static class ServiceManager {
        public static void AddMyServices(this IServiceCollection services) {
            services.AddScoped<ISpendingGraphService, SpendingGraphService>();
            services.AddScoped<IDbCleanupService, DbCleanupService>();
            services.AddScoped<IDropdownPopulatorService, DropdownPopulatorService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

            services.AddTransient<IDupeCheckerService<Transaction>, TransactionDupeCheckerService>();
        }

        public static void AddScheduledJobs(this IServiceCollection services) {
            services.AddHostedService<DailyBillToTransactionJob>();
            services.AddHostedService<RecurringTransferJob>();
        }
    }
}
