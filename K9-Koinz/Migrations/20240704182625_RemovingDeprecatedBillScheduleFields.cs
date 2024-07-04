using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class RemovingDeprecatedBillScheduleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "FirstDueDate",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "RepeatFrequency",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "RepeatFrequencyCount",
                table: "Bill");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Bill",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstDueDate",
                table: "Bill",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "RepeatFrequency",
                table: "Bill",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RepeatFrequencyCount",
                table: "Bill",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
