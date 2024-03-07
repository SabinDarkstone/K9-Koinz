using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations
{
    /// <inheritdoc />
    public partial class ChangingTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Account_AccountId",
                table: "Goals");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Account_AccountId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Bill_BillId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Category_CategoryId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Merchant_MerchantId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Tag_TagId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Transfer_TransferId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfer_Goals_SavingsGoalId",
                table: "Transfer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Goals",
                table: "Goals");

            migrationBuilder.RenameTable(
                name: "Transaction",
                newName: "BankTransaction");

            migrationBuilder.RenameTable(
                name: "Goals",
                newName: "Goal");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_TransferId",
                table: "BankTransaction",
                newName: "IX_BankTransaction_TransferId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_TagId",
                table: "BankTransaction",
                newName: "IX_BankTransaction_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_SavingsGoalId",
                table: "BankTransaction",
                newName: "IX_BankTransaction_SavingsGoalId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_MerchantId",
                table: "BankTransaction",
                newName: "IX_BankTransaction_MerchantId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_CategoryId",
                table: "BankTransaction",
                newName: "IX_BankTransaction_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_BillId",
                table: "BankTransaction",
                newName: "IX_BankTransaction_BillId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_AccountId",
                table: "BankTransaction",
                newName: "IX_BankTransaction_AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Goals_Name",
                table: "Goal",
                newName: "IX_Goal_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Goals_AccountId",
                table: "Goal",
                newName: "IX_Goal_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankTransaction",
                table: "BankTransaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Goal",
                table: "Goal",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransaction_Account_AccountId",
                table: "BankTransaction",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransaction_Bill_BillId",
                table: "BankTransaction",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransaction_Category_CategoryId",
                table: "BankTransaction",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransaction_Goal_SavingsGoalId",
                table: "BankTransaction",
                column: "SavingsGoalId",
                principalTable: "Goal",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransaction_Merchant_MerchantId",
                table: "BankTransaction",
                column: "MerchantId",
                principalTable: "Merchant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransaction_Tag_TagId",
                table: "BankTransaction",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransaction_Transfer_TransferId",
                table: "BankTransaction",
                column: "TransferId",
                principalTable: "Transfer",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Goal_Account_AccountId",
                table: "Goal",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfer_Goal_SavingsGoalId",
                table: "Transfer",
                column: "SavingsGoalId",
                principalTable: "Goal",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransaction_Account_AccountId",
                table: "BankTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTransaction_Bill_BillId",
                table: "BankTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTransaction_Category_CategoryId",
                table: "BankTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTransaction_Goal_SavingsGoalId",
                table: "BankTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTransaction_Merchant_MerchantId",
                table: "BankTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTransaction_Tag_TagId",
                table: "BankTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTransaction_Transfer_TransferId",
                table: "BankTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Goal_Account_AccountId",
                table: "Goal");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfer_Goal_SavingsGoalId",
                table: "Transfer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Goal",
                table: "Goal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BankTransaction",
                table: "BankTransaction");

            migrationBuilder.RenameTable(
                name: "Goal",
                newName: "Goals");

            migrationBuilder.RenameTable(
                name: "BankTransaction",
                newName: "Transaction");

            migrationBuilder.RenameIndex(
                name: "IX_Goal_Name",
                table: "Goals",
                newName: "IX_Goals_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Goal_AccountId",
                table: "Goals",
                newName: "IX_Goals_AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_BankTransaction_TransferId",
                table: "Transaction",
                newName: "IX_Transaction_TransferId");

            migrationBuilder.RenameIndex(
                name: "IX_BankTransaction_TagId",
                table: "Transaction",
                newName: "IX_Transaction_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_BankTransaction_SavingsGoalId",
                table: "Transaction",
                newName: "IX_Transaction_SavingsGoalId");

            migrationBuilder.RenameIndex(
                name: "IX_BankTransaction_MerchantId",
                table: "Transaction",
                newName: "IX_Transaction_MerchantId");

            migrationBuilder.RenameIndex(
                name: "IX_BankTransaction_CategoryId",
                table: "Transaction",
                newName: "IX_Transaction_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_BankTransaction_BillId",
                table: "Transaction",
                newName: "IX_Transaction_BillId");

            migrationBuilder.RenameIndex(
                name: "IX_BankTransaction_AccountId",
                table: "Transaction",
                newName: "IX_Transaction_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Goals",
                table: "Goals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Account_AccountId",
                table: "Goals",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Account_AccountId",
                table: "Transaction",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Bill_BillId",
                table: "Transaction",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Category_CategoryId",
                table: "Transaction",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Goals_SavingsGoalId",
                table: "Transaction",
                column: "SavingsGoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Merchant_MerchantId",
                table: "Transaction",
                column: "MerchantId",
                principalTable: "Merchant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Tag_TagId",
                table: "Transaction",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Transfer_TransferId",
                table: "Transaction",
                column: "TransferId",
                principalTable: "Transfer",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfer_Goals_SavingsGoalId",
                table: "Transfer",
                column: "SavingsGoalId",
                principalTable: "Goals",
                principalColumn: "Id");
        }
    }
}
