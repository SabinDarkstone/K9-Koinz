using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Models;

namespace K9_Koinz.Data {
    public class KoinzContext : DbContext {

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetLine> BudgetLines {  get; set; }

        public KoinzContext (DbContextOptions<KoinzContext> options)
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

            // Subcategories
            modelBuilder.Entity<Category>().HasMany(x => x.ChildCategories).WithOne(x => x.ParentCategory).HasForeignKey(x => x.ParentCategoryId).OnDelete(DeleteBehavior.NoAction);

            // Uniqueness
            modelBuilder.Entity<Merchant>().HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<Category>().HasIndex(x => x.Name)
                .IsUnique();

            // Converters
            modelBuilder.Entity<Transaction>().Property(x => x.Amount).HasConversion<double>();
            modelBuilder.Entity<BudgetLine>().Property(x => x.SpentAmount).HasConversion<double>();
            modelBuilder.Entity<BudgetLine>().Property(x => x.BudgetedAmount).HasConversion<double>();
            modelBuilder.Entity<Account>().Property(x => x.CurrentBalance).HasConversion<double>();
            modelBuilder.Entity<Account>().Property(x => x.InitialBalance).HasConversion<double>();
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
    }
}
