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
    [Migration("20240204210115_BillDatesFix")]
    partial class BillDatesFix
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("K9_Koinz.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<double>("InitialBalance")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateTime>("InitialBalanceDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("TEXT");

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

                    b.Property<double>("BillAmount")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("FirstDueDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastDueDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("TEXT");

                    b.Property<string>("MerchantName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("RepeatFrequency")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RepeatFrequencyCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("MerchantId");

                    b.HasIndex("Name")
                        .IsUnique();

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

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("DoNotUseCategories")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastModifiedDate")
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

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("DoRollover")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("TEXT");

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

                    b.Property<DateTime>("EndDate")
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

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ParentCategoryId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ParentCategoryName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Merchant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Merchant", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("HexColor")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ShortForm")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

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

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("TEXT");

                    b.Property<string>("CategoryName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<bool>("DoNotSkip")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("TEXT");

                    b.Property<string>("MerchantName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("TagId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("BillId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("MerchantId");

                    b.HasIndex("TagId");

                    b.ToTable("Transaction", (string)null);
                });

            modelBuilder.Entity("K9_Koinz.Models.Bill", b =>
                {
                    b.HasOne("K9_Koinz.Models.Account", "Account")
                        .WithMany("Bills")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("K9_Koinz.Models.Merchant", "Merchant")
                        .WithMany("Bills")
                        .HasForeignKey("MerchantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Merchant");
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
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("ParentCategory");
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
                        .HasForeignKey("BillId");

                    b.HasOne("K9_Koinz.Models.Category", "Category")
                        .WithMany("Transactions")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("K9_Koinz.Models.Merchant", "Merchant")
                        .WithMany("Transactions")
                        .HasForeignKey("MerchantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("K9_Koinz.Models.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId");

                    b.Navigation("Account");

                    b.Navigation("Bill");

                    b.Navigation("Category");

                    b.Navigation("Merchant");

                    b.Navigation("Tag");
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
                    b.Navigation("BudgetLines");

                    b.Navigation("ChildCategories");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("K9_Koinz.Models.Merchant", b =>
                {
                    b.Navigation("Bills");

                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
