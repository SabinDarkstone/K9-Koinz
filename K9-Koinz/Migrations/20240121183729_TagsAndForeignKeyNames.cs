using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class TagsAndForeignKeyNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPeriod_BudgetLineItem_BudgetLineId",
                table: "BudgetPeriod");

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "Transaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Transaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchantName",
                table: "Transaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagNames",
                table: "Transaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentCategoryName",
                table: "Category",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BudgetLineId",
                table: "BudgetPeriod",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BudgetCategoryName",
                table: "BudgetLineItem",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BudgetName",
                table: "BudgetLineItem",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BudgetTagId",
                table: "Budget",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BudgetTagName",
                table: "Budget",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransactionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionTag_Transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budget_BudgetTagId",
                table: "Budget",
                column: "BudgetTagId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTag_TagId",
                table: "TransactionTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTag_TransactionId",
                table: "TransactionTag",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Tag_BudgetTagId",
                table: "Budget",
                column: "BudgetTagId",
                principalTable: "Tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPeriod_BudgetLineItem_BudgetLineId",
                table: "BudgetPeriod",
                column: "BudgetLineId",
                principalTable: "BudgetLineItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Tag_BudgetTagId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetPeriod_BudgetLineItem_BudgetLineId",
                table: "BudgetPeriod");

            migrationBuilder.DropTable(
                name: "TransactionTag");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Budget_BudgetTagId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "MerchantName",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "TagNames",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ParentCategoryName",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "BudgetCategoryName",
                table: "BudgetLineItem");

            migrationBuilder.DropColumn(
                name: "BudgetName",
                table: "BudgetLineItem");

            migrationBuilder.DropColumn(
                name: "BudgetTagId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "BudgetTagName",
                table: "Budget");

            migrationBuilder.AlterColumn<Guid>(
                name: "BudgetLineId",
                table: "BudgetPeriod",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetPeriod_BudgetLineItem_BudgetLineId",
                table: "BudgetPeriod",
                column: "BudgetLineId",
                principalTable: "BudgetLineItem",
                principalColumn: "Id");
        }
    }
}
