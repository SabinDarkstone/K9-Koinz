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
            services.AddScoped<ICashflowGraphService, CashflowGraphService>();
            services.AddScoped<ITrendGraphService, TrendGraphService>();

            // Add specialty services
            services.AddScoped<IDropdownPopulatorService, DropdownPopulatorService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddTransient<IDupeCheckerService<Transaction>, TransactionDupeCheckerService>();

            // Add base repository and trigger services
            services.AddScoped(typeof(Repository<>));
            services.AddScoped<IRepoFactory, RepoFactory>();
            services.AddScoped(typeof(GenericTrigger<>));
            services.AddScoped(typeof(TriggeredRepository<>));

            // Register trigger services
            services.AddScoped<ITrigger<Transaction>, TransactionTrigger>();
            services.AddScoped<ITrigger<Bill>, BillTrigger>();
            services.AddScoped<ITrigger<Account>, GenericTrigger<Account>>();
            services.AddScoped<ITrigger<Budget>, GenericTrigger<Budget>>();
            services.AddScoped<ITrigger<BudgetLine>, GenericTrigger<BudgetLine>>();
            services.AddScoped<ITrigger<BudgetLinePeriod>, GenericTrigger<BudgetLinePeriod>>();
            services.AddScoped<ITrigger<Category>, GenericTrigger<Category>>();
            services.AddScoped<ITrigger<Merchant>, GenericTrigger<Merchant>>();
            services.AddScoped<ITrigger<RepeatConfig>, GenericTrigger<RepeatConfig>>();
            services.AddScoped<ITrigger<SavingsGoal>, GenericTrigger<SavingsGoal>>();
            services.AddScoped<ITrigger<Tag>, GenericTrigger<Tag>>();
            services.AddScoped<ITrigger<Transfer>, GenericTrigger<Transfer>>();

            // Register repository services
            services.AddScoped<TransactionRepository>();
            services.AddScoped<SavingsRepository>();
            services.AddScoped<BillRepository>();
            services.AddScoped<Repository<Account>>();
            services.AddScoped<Repository<Budget>>();
            services.AddScoped<Repository<BudgetLine>>();
            services.AddScoped<Repository<BudgetLinePeriod>>();
            services.AddScoped<Repository<Category>>();
            services.AddScoped<Repository<Merchant>>();
            services.AddScoped<Repository<RepeatConfig>>();
            services.AddScoped<Repository<Tag>>();
            services.AddScoped<Repository<Transfer>>();

        }

        public static void AddScheduledJobs(this IServiceCollection services) {
            services.AddHostedService<DailyBillToTransactionJob>();
            services.AddHostedService<RecurringTransferJob>();
        }
    }
}
