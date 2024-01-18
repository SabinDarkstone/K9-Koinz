using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class BudgetLinePeriodsForRollover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpentAmount",
                table: "BudgetLineItem");

            migrationBuilder.DropColumn(
                name: "CurrentBalance",
                table: "Account");

            migrationBuilder.AddColumn<bool>(
                name: "DoRollover",
                table: "BudgetLineItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BudgetPeriod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BudgetLineId = table.Column<Guid>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartingAmount = table.Column<double>(type: "REAL", nullable: false),
                    SpentAmount = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetPeriod_BudgetLineItem_BudgetLineId",
                        column: x => x.BudgetLineId,
                        principalTable: "BudgetLineItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPeriod_BudgetLineId",
                table: "BudgetPeriod",
                column: "BudgetLineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetPeriod");

            migrationBuilder.DropColumn(
                name: "DoRollover",
                table: "BudgetLineItem");

            migrationBuilder.AddColumn<double>(
                name: "SpentAmount",
                table: "BudgetLineItem",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CurrentBalance",
                table: "Account",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
