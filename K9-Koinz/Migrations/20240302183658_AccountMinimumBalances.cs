using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class AccountMinimumBalances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MinimumBalance",
                table: "Account",
                type: "decimal(10, 2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumBalance",
                table: "Account");
        }
    }
}
