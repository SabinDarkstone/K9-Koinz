using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class AddingInnerError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InnerException",
                table: "Error",
                newName: "InnerExceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Error_InnerExceptionId",
                table: "Error",
                column: "InnerExceptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Error_Error_InnerExceptionId",
                table: "Error",
                column: "InnerExceptionId",
                principalTable: "Error",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Error_Error_InnerExceptionId",
                table: "Error");

            migrationBuilder.DropIndex(
                name: "IX_Error_InnerExceptionId",
                table: "Error");

            migrationBuilder.RenameColumn(
                name: "InnerExceptionId",
                table: "Error",
                newName: "InnerException");
        }
    }
}
