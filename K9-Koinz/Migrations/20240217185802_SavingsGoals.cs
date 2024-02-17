using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class SavingsGoals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SavingsGoalId",
                table: "Transaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SavingsGoalName",
                table: "Transaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    TargetAmount = table.Column<double>(type: "REAL", nullable: false),
                    SavedAmount = table.Column<double>(type: "REAL", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AccountName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_SavingsGoalId",
                table: "Transaction",
                column: "SavingsGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_AccountId",
                table: "Goals",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_Name",
                table: "Goals",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction",
                column: "SavingsGoalId",
                principalTable: "Goals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_SavingsGoalId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "SavingsGoalId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "SavingsGoalName",
                table: "Transaction");
        }
    }
}
