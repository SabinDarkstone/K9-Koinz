using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class CreatedAndModifiedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Transfer",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Transfer",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Tag",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Tag",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "RepeatConfig",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "RepeatConfig",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Merchant",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Merchant",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "JobStatus",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "JobStatus",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Goal",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Goal",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Category",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Category",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "BudgetPeriod",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "BudgetPeriod",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "BudgetLineItem",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "BudgetLineItem",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Budget",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Budget",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Bill",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Bill",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "BankTransaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "BankTransaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Account",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Account",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RepeatConfig");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "RepeatConfig");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Merchant");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Merchant");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "JobStatus");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "JobStatus");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Goal");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Goal");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "BudgetPeriod");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "BudgetPeriod");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "BudgetLineItem");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "BudgetLineItem");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "BankTransaction");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "BankTransaction");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Account");
        }
    }
}
