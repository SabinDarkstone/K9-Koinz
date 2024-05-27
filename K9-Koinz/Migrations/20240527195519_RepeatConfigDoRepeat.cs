using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations {
    /// <inheritdoc />
    public partial class RepeatConfigDoRepeat : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<bool>(
                name: "DoRepeat",
                table: "RepeatConfig",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"UPDATE RepeatConfig SET DoRepeat = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn(
                name: "DoRepeat",
                table: "RepeatConfig");
        }
    }
}
