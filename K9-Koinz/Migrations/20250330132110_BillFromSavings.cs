using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class BillFromSavings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SavingsGoalId",
                table: "Bill",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SavingsGoalName",
                table: "Bill",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bill_SavingsGoalId",
                table: "Bill",
                column: "SavingsGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_Goal_SavingsGoalId",
                table: "Bill",
                column: "SavingsGoalId",
                principalTable: "Goal",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_Goal_SavingsGoalId",
                table: "Bill");

            migrationBuilder.DropIndex(
                name: "IX_Bill_SavingsGoalId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "SavingsGoalId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "SavingsGoalName",
                table: "Bill");
        }
    }
}
