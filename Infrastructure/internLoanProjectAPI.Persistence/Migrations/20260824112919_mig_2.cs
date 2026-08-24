using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace internLoanProjectAPI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "LoanApplications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicationDate",
                table: "LoanApplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionDate",
                table: "LoanApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DecisionNote",
                table: "LoanApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationDate",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "DecisionDate",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "DecisionNote",
                table: "LoanApplications");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "LoanApplications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
