using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public KoinzContext(DbContextOptions<KoinzContext> options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // Table names
            modelBuilder.Entity<Account>().ToTable("Account").HasKey(x => x.Id);
            modelBuilder.Entity<Category>().ToTable("Category").HasKey(x => x.Id);
            modelBuilder.Entity<Transaction>().ToTable("Transaction").HasKey(x => x.Id);
            modelBuilder.Entity<Budget>().ToTable("Budget").HasKey(x => x.Id);
            modelBuilder.Entity<BudgetLine>().ToTable("BudgetLineItem").HasKey(x => x.Id);
            modelBuilder.Entity<Merchant>().ToTable("Merchant").HasKey(x => x.Id);
            modelBuilder.Entity<BudgetLinePeriod>().ToTable("BudgetPeriod").HasKey(x => x.Id);
            modelBuilder.Entity<Tag>().ToTable("Tag").HasKey(x => x.Id);
            modelBuilder.Entity<Bill>().ToTable("Bill").HasKey(x => x.Id);

            // Subcategories
            modelBuilder.Entity<Category>()
                .HasMany(x => x.ChildCategories)
                .WithOne(x => x.ParentCategory)
                .HasForeignKey(x => x.ParentCategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            // Bills
            modelBuilder.Entity<Bill>()
                .HasMany(x => x.Transactions)
                .WithOne(x => x.Bill)
                .HasForeignKey(x => x.BillId)
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
        }

        public override int SaveChanges() {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is DateTrackedEntity && (
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified));

            foreach (var entityEntry in entries) {
                ((DateTrackedEntity)entityEntry.Entity).LastModifiedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added) {
                    ((DateTrackedEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is DateTrackedEntity && (
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified));

            foreach (var entityEntry in entries) {
                var dateTracked = (DateTrackedEntity)entityEntry.Entity;
                dateTracked.LastModifiedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added) {
                    dateTracked.CreatedDate = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
