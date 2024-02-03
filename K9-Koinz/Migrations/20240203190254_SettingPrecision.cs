using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class SettingPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Transaction",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryType",
                table: "Category",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "StartingAmount",
                table: "BudgetPeriod",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<double>(
                name: "SpentAmount",
                table: "BudgetPeriod",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<double>(
                name: "BudgetedAmount",
                table: "BudgetLineItem",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<double>(
                name: "InitialBalance",
                table: "Account",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Transaction",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryType",
                table: "Category",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<double>(
                name: "StartingAmount",
                table: "BudgetPeriod",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "SpentAmount",
                table: "BudgetPeriod",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "BudgetedAmount",
                table: "BudgetLineItem",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "InitialBalance",
                table: "Account",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "decimal(10, 2)");
        }
    }
}
