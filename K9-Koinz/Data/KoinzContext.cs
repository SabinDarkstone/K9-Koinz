using Microsoft.EntityFrameworkCore;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Data {
    public class KoinzContext : DbContext {

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetLine> BudgetLines { get; set; }
        public DbSet<BudgetLinePeriod> BudgetLinePeriods { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<SavingsGoal> SavingsGoals { get; set; }
        public DbSet<RepeatConfig> RepeatConfigs { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<ScheduledJobStatus> JobStatuses { get; set; }
        public DbSet<ScheduledSplit> SplitTransfers { get; set; }

        public KoinzContext(DbContextOptions<KoinzContext> options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // Table names
            modelBuilder.Entity<Account>().ToTable("Account").HasKey(x => x.Id);
            modelBuilder.Entity<Category>().ToTable("Category").HasKey(x => x.Id);
            modelBuilder.Entity<Transaction>().ToTable("BankTransaction").HasKey(x => x.Id);
            modelBuilder.Entity<Budget>().ToTable("Budget").HasKey(x => x.Id);
            modelBuilder.Entity<BudgetLine>().ToTable("BudgetLineItem").HasKey(x => x.Id);
            modelBuilder.Entity<Merchant>().ToTable("Merchant").HasKey(x => x.Id);
            modelBuilder.Entity<BudgetLinePeriod>().ToTable("BudgetPeriod").HasKey(x => x.Id);
            modelBuilder.Entity<Tag>().ToTable("Tag").HasKey(x => x.Id);
            modelBuilder.Entity<Bill>().ToTable("Bill").HasKey(x => x.Id);
            modelBuilder.Entity<SavingsGoal>().ToTable("Goal").HasKey(x => x.Id);
            modelBuilder.Entity<RepeatConfig>().ToTable("RepeatConfig").HasKey(x => x.Id);
            modelBuilder.Entity<Transfer>().ToTable("Transfer").HasKey(x => x.Id);
            modelBuilder.Entity<ScheduledJobStatus>().ToTable("JobStatus").HasKey(x => x.Id);
            modelBuilder.Entity<ScheduledSplit>().ToTable("ScheduleSplit").HasKey(x => x.Id);

            // Subcategories
            modelBuilder.Entity<Category>()
                .HasMany(x => x.ChildCategories)
                .WithOne(x => x.ParentCategory)
                .HasForeignKey(x => x.ParentCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Bills
            modelBuilder.Entity<Bill>()
                .HasMany(x => x.Transactions)
                .WithOne(x => x.Bill)
                .HasForeignKey(x => x.BillId)
                .OnDelete(DeleteBehavior.SetNull);

            // Savings Goals
            modelBuilder.Entity<SavingsGoal>()
                .HasMany(x => x.Transactions)
                .WithOne(x => x.SavingsGoal)
                .HasForeignKey(x => x.SavingsGoalId)
                .OnDelete(DeleteBehavior.SetNull);

            // Transfers
            modelBuilder.Entity<Transfer>()
                .HasMany(x => x.Transactions)
                .WithOne(x => x.Transfer)
                .HasForeignKey(x => x.TransferId)
                .OnDelete(DeleteBehavior.SetNull);

            // Tags
            modelBuilder.Entity<Tag>()
                .HasMany(x => x.Transactions)
                .WithOne(x => x.Tag)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.SetNull);

            // Uniqueness
            modelBuilder.Entity<Merchant>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<Category>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<Tag>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<Bill>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<SavingsGoal>()
                .HasIndex(x => x.Name)
                .IsUnique();

            // Budget -> Budget Line
            modelBuilder.Entity<Budget>()
                .HasMany(x => x.BudgetLines)
                .WithOne(x => x.Budget)
                .HasForeignKey(x => x.BudgetId);

            // Recurring Transfer -> Instance Transfers
            modelBuilder.Entity<Transfer>()
                .HasOne(x => x.RecurringTransfer)
                .WithMany(x => x.InstantiatedFromRecurring)
                .HasForeignKey(x => x.RecurringTransferId)
                .OnDelete(DeleteBehavior.Cascade);

            // Scheduled Transfer -> Split Lines
            modelBuilder.Entity<Transfer>()
                .HasMany(x => x.SplitLines)
                .WithOne(x => x.ParentTransfer)
                .HasForeignKey(x => x.ParentTransferId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override int SaveChanges() {
            SetDateFields();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess) {
            SetDateFields();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) {
            SetDateFields();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            SetDateFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetDateFields() {
            var now = DateTime.Now;

            foreach (var changedEntity in ChangeTracker.Entries()) {
                if (changedEntity.Entity is BaseEntity entity) {
                    switch (changedEntity.State) {
                        case EntityState.Added:
                            entity.CreatedDate = now;
                            entity.LastModifiedDate = now;
                            break;
                        case EntityState.Modified:
                            Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                            entity.LastModifiedDate = now;
                            break;
                    }
                }
            }
        }
    }
}
