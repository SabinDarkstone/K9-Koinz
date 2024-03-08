using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations {
    /// <inheritdoc />
    public partial class TagsUpdate : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransaction_Tag_TagId",
                table: "BankTransaction");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransaction_Tag_TagId",
                table: "BankTransaction",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {

            migrationBuilder.DropForeignKey(
                name: "FK_BankTransaction_Tag_TagId",
                table: "BankTransaction");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransaction_Tag_TagId",
                table: "BankTransaction",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id");
        }
    }
}
