using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class NoCategoryBudgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DoNotUseCategories",
                table: "Budget",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoNotUseCategories",
                table: "Budget");
        }
    }
}
