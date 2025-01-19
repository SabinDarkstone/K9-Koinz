using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreToErrorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "Error",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InnerException",
                table: "Error",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Error",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StackTraceString",
                table: "Error",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "Error");

            migrationBuilder.DropColumn(
                name: "InnerException",
                table: "Error");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Error");

            migrationBuilder.DropColumn(
                name: "StackTraceString",
                table: "Error");
        }
    }
}
