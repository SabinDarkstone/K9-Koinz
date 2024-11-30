using K9_Koinz.Data;
using K9_Koinz.Data.Repositories;
using K9_Koinz.Data.Repositories.Meta;
using K9_Koinz.Factories;
using K9_Koinz.Models;
using K9_Koinz.Services;
using K9_Koinz.Triggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace K9_Tests {
    public class TestFixture : IDisposable {

        private static readonly Dictionary<Type, Type> TRIGGER_LIST = new() {
            { typeof(Transaction), typeof(TransactionTrigger) },
            { typeof(Bill), typeof(BillTrigger) }
        };

        private static readonly Dictionary<Type, Type> REPOSITORY_LIST = new() {
            { typeof(Account), typeof(Repository<Account>) },
            { typeof(Bill), typeof(BillRepository) },
            { typeof(Budget), typeof(Repository<Budget>) },
            { typeof(BudgetLine), typeof(Repository<BudgetLine>) },
            { typeof(BudgetLinePeriod), typeof(Repository<BudgetLinePeriod>) },
            { typeof(Category), typeof(Repository<Category>) },
            { typeof(Merchant), typeof(Repository<Merchant>) },
            { typeof(RepeatConfig), typeof(Repository<RepeatConfig>) },
            { typeof(SavingsGoal), typeof(SavingsRepository) },
            { typeof(Tag), typeof(Repository<Tag>) },
            { typeof(Transaction), typeof(TransactionRepository) },
            { typeof(Transfer), typeof(Repository<Transfer>) }
        };

        public ServiceProvider ServiceProvider { get; private set; }
        public KoinzContext DbContext { get; private set; }
        public DataFactory DataFactory { get; private set; }

        public TestFixture() {
            var services = new ServiceCollection();

            // Register database in memory context
            services.AddDbContext<KoinzContext>(options => {
                options.UseInMemoryDatabase("TestDb");
            });

            // Add generic services
            services.AddLogging();

            // Add specialty services
            services.AddScoped<IDropdownPopulatorService, DropdownPopulatorService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddTransient<IDupeCheckerService<Transaction>, TransactionDupeCheckerService>();

            // Add base repository and trigger services
            services.AddScoped(typeof(Repository<>));
            services.AddScoped<IRepoFactory, RepoFactory>();
            services.AddScoped(typeof(GenericTrigger<>));
            services.AddScoped(typeof(TriggeredRepository<>));

            // Add data factory
            services.AddScoped<DataFactory>();

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

            // Build service provider
            ServiceProvider = services.BuildServiceProvider();

            DbContext = ServiceProvider.GetRequiredService<KoinzContext>();
            DataFactory = ServiceProvider.GetRequiredService<DataFactory>();

            SeedDatabase();
        }

        private void SeedDatabase() {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KoinzContext>();

            DbInitializer.Initialize(context);
        }

        public void Dispose() {
            if (ServiceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
}
