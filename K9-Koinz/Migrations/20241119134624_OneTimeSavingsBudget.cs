using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class OneTimeSavingsBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CountAgainstBudget",
                table: "BankTransaction",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE BankTransaction as t SET CountAgainstBudget = 1 FROM BankTransaction WHERE t.Id = (SELECT t.Id FROM BankTransaction INNER JOIN Transfer as f ON t.TransferId = f.Id WHERE t.TransferId != \"\" AND t.SavingsGoalId != \"\" AND t.IsSavingsSpending = 0 AND f.RecurringTransferId IS NULL AND t.Amount > 0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountAgainstBudget",
                table: "BankTransaction");
        }
    }
}
