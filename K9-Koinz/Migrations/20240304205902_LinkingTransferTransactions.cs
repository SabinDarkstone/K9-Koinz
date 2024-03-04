using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class LinkingTransferTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Category_ParentCategoryId",
                table: "Category");

            migrationBuilder.DropTable(
                name: "RepeatTransfer");

            migrationBuilder.AddColumn<Guid>(
                name: "TransferId",
                table: "Transaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Transfer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FromAccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ToAccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MerchantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<double>(type: "decimal(10, 2)", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SavingsGoalId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RepeatConfigId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfer_Account_FromAccountId",
                        column: x => x.FromAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Account_ToAccountId",
                        column: x => x.ToAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Goals_SavingsGoalId",
                        column: x => x.SavingsGoalId,
                        principalTable: "Goals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transfer_Merchant_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_RepeatConfig_RepeatConfigId",
                        column: x => x.RepeatConfigId,
                        principalTable: "RepeatConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transfer_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TransferId",
                table: "Transaction",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_CategoryId",
                table: "Transfer",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_FromAccountId",
                table: "Transfer",
                column: "FromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_MerchantId",
                table: "Transfer",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_RepeatConfigId",
                table: "Transfer",
                column: "RepeatConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_SavingsGoalId",
                table: "Transfer",
                column: "SavingsGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_TagId",
                table: "Transfer",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_ToAccountId",
                table: "Transfer",
                column: "ToAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Category_ParentCategoryId",
                table: "Category",
                column: "ParentCategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Transfer_TransferId",
                table: "Transaction",
                column: "TransferId",
                principalTable: "Transfer",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Category_ParentCategoryId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Transfer_TransferId",
                table: "Transaction");

            migrationBuilder.DropTable(
                name: "Transfer");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_TransferId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "TransferId",
                table: "Transaction");

            migrationBuilder.CreateTable(
                name: "RepeatTransfer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FromAccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MerchantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RepeatConfigId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SavingsGoalId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ToAccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<double>(type: "decimal(10, 2)", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepeatTransfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepeatTransfer_Account_FromAccountId",
                        column: x => x.FromAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepeatTransfer_Account_ToAccountId",
                        column: x => x.ToAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepeatTransfer_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepeatTransfer_Goals_SavingsGoalId",
                        column: x => x.SavingsGoalId,
                        principalTable: "Goals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RepeatTransfer_Merchant_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepeatTransfer_RepeatConfig_RepeatConfigId",
                        column: x => x.RepeatConfigId,
                        principalTable: "RepeatConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepeatTransfer_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepeatTransfer_CategoryId",
                table: "RepeatTransfer",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RepeatTransfer_FromAccountId",
                table: "RepeatTransfer",
                column: "FromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RepeatTransfer_MerchantId",
                table: "RepeatTransfer",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_RepeatTransfer_RepeatConfigId",
                table: "RepeatTransfer",
                column: "RepeatConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_RepeatTransfer_SavingsGoalId",
                table: "RepeatTransfer",
                column: "SavingsGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_RepeatTransfer_TagId",
                table: "RepeatTransfer",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_RepeatTransfer_ToAccountId",
                table: "RepeatTransfer",
                column: "ToAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Category_ParentCategoryId",
                table: "Category",
                column: "ParentCategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }
    }
}
