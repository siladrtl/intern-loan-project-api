using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace internLoanProjectAPI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerRegistrationId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerRegistrationId",
                table: "Customers",
                column: "CustomerRegistrationId",
                unique: true,
                filter: "[CustomerRegistrationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CustomerRegistrations_CustomerRegistrationId",
                table: "Customers",
                column: "CustomerRegistrationId",
                principalTable: "CustomerRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CustomerRegistrations_CustomerRegistrationId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CustomerRegistrationId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerRegistrationId",
                table: "Customers");
        }
    }
}
