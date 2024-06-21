using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingApp.Migrations
{
    /// <inheritdoc />
    public partial class TransactionToReportDataItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "reportDataItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_reportDataItems_TransactionId",
                table: "reportDataItems",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_reportDataItems_Transactions_TransactionId",
                table: "reportDataItems",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reportDataItems_Transactions_TransactionId",
                table: "reportDataItems");

            migrationBuilder.DropIndex(
                name: "IX_reportDataItems_TransactionId",
                table: "reportDataItems");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "reportDataItems");
        }
    }
}
