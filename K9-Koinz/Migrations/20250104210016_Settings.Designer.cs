﻿// <auto-generated />
using System;
using K9_Koinz.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace K9_Koinz.Migrations
{
    [DbContext(typeof(KoinzContext))]
    [Migration("20250104210016_Settings")]
    partial class Settings
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("K9_Koinz.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("HideAccountTransactions")
                        .HasColumnType("INTEGER");

                    b.Property<double>("InitialBalance")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateTime>("InitialBalanceDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRetired")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<double?>("MinimumBalance")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Account", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Bill", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AccountName")
                        .HasColumnType("TEXT");

                    b.Property<double>("Amount")
                        .HasColumnType("REAL");

                    b.Property<Guid?>("CategoryId")
                        .HasColumnType("TEXT");

                    b.Property<string>("CategoryName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAutopay")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRepeatBill")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastDueDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("TEXT");

                    b.Property<string>("MerchantName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("RepeatConfigId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("MerchantId");

                    b.HasIndex("RepeatConfigId");

                    b.ToTable("Bill", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Budget", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("BudgetTagId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BudgetTagName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("DoNotUseCategories")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("SortOrder")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Timespan")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BudgetTagId");

                    b.ToTable("Budget", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.BudgetLine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BudgetCategoryId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BudgetCategoryName")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BudgetId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BudgetName")
                        .HasColumnType("TEXT");

                    b.Property<double>("BudgetedAmount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("DoRollover")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("GreenBarAlways")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("ShowWeeklyLines")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BudgetCategoryId");

                    b.HasIndex("BudgetId");

                    b.ToTable("BudgetLineItem", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.BudgetLinePeriod", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BudgetLineId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<double>("SpentAmount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<double>("StartingAmount")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("Id");

                    b.HasIndex("BudgetLineId");

                    b.ToTable("BudgetPeriod", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("CategoryType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("FontAwesomeIcon")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRetired")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ParentCategoryId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ParentCategoryName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Merchant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRetired")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Merchant", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.RepeatConfig", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("DoRepeat")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FirstFiring")
                        .HasColumnType("TEXT");

                    b.Property<int>("Frequency")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("IntervalGap")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Mode")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("NextFiring")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("PreviousFiring")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("TerminationDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("RepeatConfig", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.SavingsGoal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AccountName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<double>("SavedAmount")
                        .HasColumnType("REAL");

                    b.Property<int>("SavingsType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<double?>("StartingAmount")
                        .HasColumnType("REAL");

                    b.Property<double?>("TargetAmount")
                        .HasColumnType("REAL");

                    b.Property<DateTime?>("TargetDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Goal", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.ScheduledJobStatus", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("ErrorMessages")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("JobName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("NextRunTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("StackTrace")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("JobStatus", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Setting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("DefaultValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Setting", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("HexColor")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRetired")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ShortForm")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tag", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AccountName")
                        .HasColumnType("TEXT");

                    b.Property<double>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<Guid?>("BillId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("CategoryId")
                        .HasColumnType("TEXT");

                    b.Property<string>("CategoryName")
                        .HasColumnType("TEXT");

                    b.Property<bool>("CountAgainstBudget")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<bool>("DoNotSkip")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSavingsSpending")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSplit")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("TEXT");

                    b.Property<string>("MerchantName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ParentTransactionId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("SavingsGoalId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SavingsGoalName")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("TagId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("TransferId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("BillId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("MerchantId");

                    b.HasIndex("ParentTransactionId");

                    b.HasIndex("SavingsGoalId");

                    b.HasIndex("TagId");

                    b.HasIndex("TransferId");

                    b.ToTable("BankTransaction", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Transfer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<double>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("FromAccountId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSplit")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsTransferFromBudget")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("RecurringTransferId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("RepeatConfigId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("SavingsGoalId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("TagId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ToAccountId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("FromAccountId");

                    b.HasIndex("MerchantId");

                    b.HasIndex("RecurringTransferId");

                    b.HasIndex("RepeatConfigId");

                    b.HasIndex("SavingsGoalId");

                    b.HasIndex("TagId");

                    b.HasIndex("ToAccountId");

                    b.ToTable("Transfer", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Bill", b =>
                {
                    b.HasOne("K9_Koinz.Models.Account", "Account")
                        .WithMany("Bills")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("K9_Koinz.Models.Category", "Category")
                        .WithMany("Bills")
                        .HasForeignKey("CategoryId");

                    b.HasOne("K9_Koinz.Models.Merchant", "Merchant")
                        .WithMany("Bills")
                        .HasForeignKey("MerchantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("K9_Koinz.Models.RepeatConfig", "RepeatConfig")
                        .WithMany()
                        .HasForeignKey("RepeatConfigId");

                    b.Navigation("Account");

                    b.Navigation("Category");

                    b.Navigation("Merchant");

                    b.Navigation("RepeatConfig");
                });

            modelBuilder.Entity("K9_Koinz.Models.Budget", b =>
                {
                    b.HasOne("K9_Koinz.Models.Tag", "BudgetTag")
                        .WithMany()
                        .HasForeignKey("BudgetTagId");

                    b.Navigation("BudgetTag");
                });

            modelBuilder.Entity("K9_Koinz.Models.BudgetLine", b =>
                {
                    b.HasOne("K9_Koinz.Models.Category", "BudgetCategory")
                        .WithMany("BudgetLines")
                        .HasForeignKey("BudgetCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("K9_Koinz.Models.Budget", "Budget")
                        .WithMany("BudgetLines")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Budget");

                    b.Navigation("BudgetCategory");
                });

            modelBuilder.Entity("K9_Koinz.Models.BudgetLinePeriod", b =>
                {
                    b.HasOne("K9_Koinz.Models.BudgetLine", "BudgetLine")
                        .WithMany("Periods")
                        .HasForeignKey("BudgetLineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BudgetLine");
                });

            modelBuilder.Entity("K9_Koinz.Models.Category", b =>
                {
                    b.HasOne("K9_Koinz.Models.Category", "ParentCategory")
                        .WithMany("ChildCategories")
                        .HasForeignKey("ParentCategoryId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("K9_Koinz.Models.SavingsGoal", b =>
                {
                    b.HasOne("K9_Koinz.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("K9_Koinz.Models.Transaction", b =>
                {
                    b.HasOne("K9_Koinz.Models.Account", "Account")
                        .WithMany("Transactions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("K9_Koinz.Models.Bill", "Bill")
                        .WithMany("Transactions")
                        .HasForeignKey("BillId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("K9_Koinz.Models.Category", "Category")
                        .WithMany("Transactions")
                        .HasForeignKey("CategoryId");

                    b.HasOne("K9_Koinz.Models.Merchant", "Merchant")
                        .WithMany("Transactions")
                        .HasForeignKey("MerchantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("K9_Koinz.Models.Transaction", "ParentTransaction")
                        .WithMany("SplitTransactions")
                        .HasForeignKey("ParentTransactionId");

                    b.HasOne("K9_Koinz.Models.SavingsGoal", "SavingsGoal")
                        .WithMany("Transactions")
                        .HasForeignKey("SavingsGoalId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("K9_Koinz.Models.Tag", "Tag")
                        .WithMany("Transactions")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("K9_Koinz.Models.Transfer", "Transfer")
                        .WithMany("Transactions")
                        .HasForeignKey("TransferId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Account");

                    b.Navigation("Bill");

                    b.Navigation("Category");

                    b.Navigation("Merchant");

                    b.Navigation("ParentTransaction");

                    b.Navigation("SavingsGoal");

                    b.Navigation("Tag");

                    b.Navigation("Transfer");
                });

            modelBuilder.Entity("K9_Koinz.Models.Transfer", b =>
                {
                    b.HasOne("K9_Koinz.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("K9_Koinz.Models.Account", "FromAccount")
                        .WithMany()
                        .HasForeignKey("FromAccountId");

                    b.HasOne("K9_Koinz.Models.Merchant", "Merchant")
                        .WithMany()
                        .HasForeignKey("MerchantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("K9_Koinz.Models.Transfer", "RecurringTransfer")
                        .WithMany("InstantiatedFromRecurring")
                        .HasForeignKey("RecurringTransferId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("K9_Koinz.Models.RepeatConfig", "RepeatConfig")
                        .WithMany()
                        .HasForeignKey("RepeatConfigId");

                    b.HasOne("K9_Koinz.Models.SavingsGoal", "SavingsGoal")
                        .WithMany()
                        .HasForeignKey("SavingsGoalId");

                    b.HasOne("K9_Koinz.Models.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId");

                    b.HasOne("K9_Koinz.Models.Account", "ToAccount")
                        .WithMany()
                        .HasForeignKey("ToAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("FromAccount");

                    b.Navigation("Merchant");

                    b.Navigation("RecurringTransfer");

                    b.Navigation("RepeatConfig");

                    b.Navigation("SavingsGoal");

                    b.Navigation("Tag");

                    b.Navigation("ToAccount");
                });

            modelBuilder.Entity("K9_Koinz.Models.Account", b =>
                {
                    b.Navigation("Bills");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("K9_Koinz.Models.Bill", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("K9_Koinz.Models.Budget", b =>
                {
                    b.Navigation("BudgetLines");
                });

            modelBuilder.Entity("K9_Koinz.Models.BudgetLine", b =>
                {
                    b.Navigation("Periods");
                });

            modelBuilder.Entity("K9_Koinz.Models.Category", b =>
                {
                    b.Navigation("Bills");

                    b.Navigation("BudgetLines");

                    b.Navigation("ChildCategories");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("K9_Koinz.Models.Merchant", b =>
                {
                    b.Navigation("Bills");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("K9_Koinz.Models.SavingsGoal", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("K9_Koinz.Models.Tag", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("K9_Koinz.Models.Transaction", b =>
                {
                    b.Navigation("SplitTransactions");
                });

            modelBuilder.Entity("K9_Koinz.Models.Transfer", b =>
                {
                    b.Navigation("InstantiatedFromRecurring");

                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
