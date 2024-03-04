using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class RepeatingBehaviorUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Merchant");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Merchant");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "BudgetLineItem");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "BudgetLineItem");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Account");

            //migrationBuilder.RenameColumn(
            //    name: "LastDueDate",
            //    table: "Bill",
            //    newName: "RepeatConfigId");

            migrationBuilder.AddColumn<Guid>(
                name: "RepeatConfigId",
                table: "Bill",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RepeatConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Mode = table.Column<int>(type: "INTEGER", nullable: false),
                    Frequency = table.Column<int>(type: "INTEGER", nullable: false),
                    IntervalGap = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstFiring = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TerminationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastFiring = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepeatConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepeatTransfer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RepeatConfigId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FromAccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ToAccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MerchantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<double>(type: "decimal(10, 2)", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: true)
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
                name: "IX_Bill_RepeatConfigId",
                table: "Bill",
                column: "RepeatConfigId");

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
                name: "IX_RepeatTransfer_TagId",
                table: "RepeatTransfer",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_RepeatTransfer_ToAccountId",
                table: "RepeatTransfer",
                column: "ToAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_RepeatConfig_RepeatConfigId",
                table: "Bill",
                column: "RepeatConfigId",
                principalTable: "RepeatConfig",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction",
                column: "SavingsGoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_RepeatConfig_RepeatConfigId",
                table: "Bill");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction");

            migrationBuilder.DropTable(
                name: "RepeatTransfer");

            migrationBuilder.DropTable(
                name: "RepeatConfig");

            migrationBuilder.DropIndex(
                name: "IX_Bill_RepeatConfigId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "IsSavingsSpending",
                table: "Transaction");

            migrationBuilder.RenameColumn(
                name: "RepeatConfigId",
                table: "Bill",
                newName: "LastDueDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Transaction",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Transaction",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Merchant",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Merchant",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Category",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Category",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "BudgetLineItem",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "BudgetLineItem",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Budget",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Budget",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Account",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Account",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction",
                column: "SavingsGoalId",
                principalTable: "Goals",
                principalColumn: "Id");
        }
    }
}
