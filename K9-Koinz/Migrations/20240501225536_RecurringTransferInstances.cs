using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class RecurringTransferInstances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RecurringTransferId",
                table: "Transfer",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_RecurringTransferId",
                table: "Transfer",
                column: "RecurringTransferId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfer_Transfer_RecurringTransferId",
                table: "Transfer",
                column: "RecurringTransferId",
                principalTable: "Transfer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfer_Transfer_RecurringTransferId",
                table: "Transfer");

            migrationBuilder.DropIndex(
                name: "IX_Transfer_RecurringTransferId",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "RecurringTransferId",
                table: "Transfer");
        }
    }
}
