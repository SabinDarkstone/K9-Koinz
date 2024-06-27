using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K9_Koinz.Migrations {
    /// <inheritdoc />
    public partial class SetRepeatBillValue : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql(
                @"
                    UPDATE Bill
                    SET IsRepeatBill = 1
                    WHERE RepeatConfigId IS NOT NULL;
                "
            ); ;
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {

        }
    }
}
