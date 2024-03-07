using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class SplitTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentTransactionId",
                table: "BankTransaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankTransaction_ParentTransactionId",
                table: "BankTransaction",
                column: "ParentTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransaction_BankTransaction_ParentTransactionId",
                table: "BankTransaction",
                column: "ParentTransactionId",
                principalTable: "BankTransaction",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransaction_BankTransaction_ParentTransactionId",
                table: "BankTransaction");

            migrationBuilder.DropIndex(
                name: "IX_BankTransaction_ParentTransactionId",
                table: "BankTransaction");

            migrationBuilder.DropColumn(
                name: "ParentTransactionId",
                table: "BankTransaction");
        }
    }
}
