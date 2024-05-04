using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class RefactoringDatesForRepeatConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastFiring",
                table: "RepeatConfig",
                newName: "PreviousFiring");

            migrationBuilder.AlterColumn<string>(
                name: "HexColor",
                table: "Tag",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextFiring",
                table: "RepeatConfig",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextFiring",
                table: "RepeatConfig");

            migrationBuilder.RenameColumn(
                name: "PreviousFiring",
                table: "RepeatConfig",
                newName: "LastFiring");

            migrationBuilder.AlterColumn<string>(
                name: "HexColor",
                table: "Tag",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
