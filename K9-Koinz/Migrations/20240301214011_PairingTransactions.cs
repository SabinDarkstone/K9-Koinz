using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class PairingTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PairedTransactionId",
                table: "Transaction",
                column: "PairedTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction",
                column: "SavingsGoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Transaction_PairedTransactionId",
                table: "Transaction",
                column: "PairedTransactionId",
                principalTable: "Transaction",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Transaction_PairedTransactionId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_PairedTransactionId",
                table: "Transaction");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction",
                column: "SavingsGoalId",
                principalTable: "Goals",
                principalColumn: "Id");
        }
    }
}
