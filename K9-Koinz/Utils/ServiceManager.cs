using K9_Koinz.Data;
using K9_Koinz.Data.Repositories;
using K9_Koinz.Data.Repositories.Meta;
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
            services.AddScoped<ITrigger<Merchant>, MerchantTrigger>();
            services.AddScoped<ITrigger<Account>, AccountTrigger>();
            services.AddScoped<ITrigger<Category>, CategoryTrigger>();
            services.AddScoped<ITrigger<Budget>, BudgetTrigger>();
            services.AddScoped<ITrigger<BudgetLine>, GenericTrigger<BudgetLine>>();
            services.AddScoped<ITrigger<BudgetLinePeriod>, GenericTrigger<BudgetLinePeriod>>();
            services.AddScoped<ITrigger<RepeatConfig>, GenericTrigger<RepeatConfig>>();
            services.AddScoped<ITrigger<SavingsGoal>, SavingsTrigger>();
            services.AddScoped<ITrigger<Tag>, GenericTrigger<Tag>>();
            services.AddScoped<ITrigger<Transfer>, GenericTrigger<Transfer>>();

            // Register repository services
            services.AddScoped<TransactionRepository>();
            services.AddScoped<SavingsRepository>();
            services.AddScoped<BillRepository>();
            services.AddScoped<MerchantRepository>();
            services.AddScoped<AccountRepository>();
            services.AddScoped<CategoryRepository>();
            services.AddScoped<BudgetRepository>();
            services.AddScoped<TagRepository>();
            services.AddScoped<Repository<BudgetLine>>();
            services.AddScoped<Repository<BudgetLinePeriod>>();
            services.AddScoped<Repository<RepeatConfig>>();
            services.AddScoped<Repository<Transfer>>();

        }

        public static void AddScheduledJobs(this IServiceCollection services) {
            services.AddHostedService<DailyBillToTransactionJob>();
            services.AddHostedService<RecurringTransferJob>();
        }
    }
}
