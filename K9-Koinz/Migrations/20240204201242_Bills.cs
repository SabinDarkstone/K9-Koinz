using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class Bills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BillId",
                table: "Transaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Bill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    AccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AccountName = table.Column<string>(type: "TEXT", nullable: true),
                    MerchantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MerchantName = table.Column<string>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RepeatFrequency = table.Column<int>(type: "INTEGER", nullable: false),
                    RepeatFrequencyCount = table.Column<int>(type: "INTEGER", nullable: false),
                    BillAmount = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bill_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bill_Merchant_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_BillId",
                table: "Transaction",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Name",
                table: "Tag",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bill_AccountId",
                table: "Bill",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_MerchantId",
                table: "Bill",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_Name",
                table: "Bill",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Bill_BillId",
                table: "Transaction",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Bill_BillId",
                table: "Transaction");

            migrationBuilder.DropTable(
                name: "Bill");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_BillId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Tag_Name",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "BillId",
                table: "Transaction");
        }
    }
}
