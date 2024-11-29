using K9_Koinz.Data;
using K9_Koinz.Factories;
using K9_Koinz.Models;
using K9_Koinz.Services;
using K9_Koinz.Services.BackgroundWorkers;
using K9_Koinz.Triggers;

namespace K9_Koinz.Utils {
    public static class ServiceManager {
        public static void AddMyServices(this IServiceCollection services) {
            services.AddScoped<ISpendingGraphService, SpendingGraphService>();
            services.AddScoped<IDbCleanupService, DbCleanupService>();
            services.AddScoped<IDropdownPopulatorService, DropdownPopulatorService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddScoped<ICashflowGraphService, CashflowGraphService>();
            services.AddScoped<ITrendGraphService, TrendGraphService>();

            services.AddTransient<IDupeCheckerService<Transaction>, TransactionDupeCheckerService>();
        }

        public static void AddScheduledJobs(this IServiceCollection services) {
            services.AddHostedService<DailyBillToTransactionJob>();
            services.AddHostedService<RecurringTransferJob>();
        }

        public static void AddDataServices(this IServiceCollection services) {
            services.AddLogging();

            services.AddScoped(typeof(Repository<>));

            services.AddScoped<IRepoFactory, RepoFactory>();
            services.AddScoped<TransactionRepository>();
            services.AddScoped<SavingsRepository>();
            services.AddScoped<BillRepository>();

            services.AddScoped(typeof(TriggeredRepository<>));
            services.AddScoped<ITrigger<Transaction>, TransactionTrigger>();
        }
    }
}
