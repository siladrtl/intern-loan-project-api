using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace internLoanProjectAPI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationDocuments_Customers_CustomerId",
                table: "VerificationDocuments");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "VerificationDocuments",
                newName: "CustomerRegistrationId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationDocuments_CustomerId",
                table: "VerificationDocuments",
                newName: "IX_VerificationDocuments_CustomerRegistrationId");

            migrationBuilder.CreateTable(
                name: "CustomerRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRegistrations", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationDocuments_CustomerRegistrations_CustomerRegistrationId",
                table: "VerificationDocuments",
                column: "CustomerRegistrationId",
                principalTable: "CustomerRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationDocuments_CustomerRegistrations_CustomerRegistrationId",
                table: "VerificationDocuments");

            migrationBuilder.DropTable(
                name: "CustomerRegistrations");

            migrationBuilder.RenameColumn(
                name: "CustomerRegistrationId",
                table: "VerificationDocuments",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationDocuments_CustomerRegistrationId",
                table: "VerificationDocuments",
                newName: "IX_VerificationDocuments_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationDocuments_Customers_CustomerId",
                table: "VerificationDocuments",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
