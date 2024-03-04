using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations {
    /// <inheritdoc />
    public partial class SavingsGoalsForRepeatTransfers : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<Guid>(
                name: "SavingsGoalId",
                table: "RepeatTransfer",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepeatTransfer_SavingsGoalId",
                table: "RepeatTransfer",
                column: "SavingsGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_RepeatTransfer_Goals_SavingsGoalId",
                table: "RepeatTransfer",
                column: "SavingsGoalId",
                principalTable: "Goals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_RepeatTransfer_Goals_SavingsGoalId",
                table: "RepeatTransfer");

            migrationBuilder.DropIndex(
                name: "IX_RepeatTransfer_SavingsGoalId",
                table: "RepeatTransfer");

            migrationBuilder.DropColumn(
                name: "SavingsGoalId",
                table: "RepeatTransfer");
        }
    }
}
