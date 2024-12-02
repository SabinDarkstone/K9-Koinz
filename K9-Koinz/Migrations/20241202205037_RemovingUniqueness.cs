using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class RemovingUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tag_Name",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Merchant_Name",
                table: "Merchant");

            migrationBuilder.DropIndex(
                name: "IX_Goal_Name",
                table: "Goal");

            migrationBuilder.DropIndex(
                name: "IX_Category_Name",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Bill_Name",
                table: "Bill");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tag_Name",
                table: "Tag",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Merchant_Name",
                table: "Merchant",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Goal_Name",
                table: "Goal",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                table: "Category",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bill_Name",
                table: "Bill",
                column: "Name",
                unique: true);
        }
    }
}
