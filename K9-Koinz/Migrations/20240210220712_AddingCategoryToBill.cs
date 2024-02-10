using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class AddingCategoryToBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Bill",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Bill",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bill_CategoryId",
                table: "Bill",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_Category_CategoryId",
                table: "Bill",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_Category_CategoryId",
                table: "Bill");

            migrationBuilder.DropIndex(
                name: "IX_Bill_CategoryId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Bill");
        }
    }
}
