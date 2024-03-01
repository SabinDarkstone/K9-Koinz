using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class RemovingPairedTransactionsForNow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Transaction_PairedTransactionId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_PairedTransactionId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "PairedTransactionId",
                table: "Transaction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PairedTransactionId",
                table: "Transaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PairedTransactionId",
                table: "Transaction",
                column: "PairedTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Transaction_PairedTransactionId",
                table: "Transaction",
                column: "PairedTransactionId",
                principalTable: "Transaction",
                principalColumn: "Id");
        }
    }
}
