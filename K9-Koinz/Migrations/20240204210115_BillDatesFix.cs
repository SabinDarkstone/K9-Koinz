using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class BillDatesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Bill",
                newName: "FirstDueDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDueDate",
                table: "Bill",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDueDate",
                table: "Bill");

            migrationBuilder.RenameColumn(
                name: "FirstDueDate",
                table: "Bill",
                newName: "StartDate");
        }
    }
}
