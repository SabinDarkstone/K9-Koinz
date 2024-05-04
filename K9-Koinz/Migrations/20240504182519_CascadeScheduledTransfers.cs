using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class CascadeScheduledTransfers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfer_Transfer_RecurringTransferId",
                table: "Transfer");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfer_Transfer_RecurringTransferId",
                table: "Transfer",
                column: "RecurringTransferId",
                principalTable: "Transfer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfer_Transfer_RecurringTransferId",
                table: "Transfer");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfer_Transfer_RecurringTransferId",
                table: "Transfer",
                column: "RecurringTransferId",
                principalTable: "Transfer",
                principalColumn: "Id");
        }
    }
}
