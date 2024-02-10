using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class BillsDontDeleteTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Bill_BillId",
                table: "Transaction");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Bill_BillId",
                table: "Transaction",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Bill_BillId",
                table: "Transaction");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Bill_BillId",
                table: "Transaction",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id");
        }
    }
}
