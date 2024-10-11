using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiberaryManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class MajorUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "Penalty",
                table: "Checkouts");

            migrationBuilder.RenameColumn(
                name: "IssuedDate",
                table: "Penalties",
                newName: "ReturnDate");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Penalties",
                newName: "PenaltyAmount");

            migrationBuilder.RenameColumn(
                name: "PenaltyId",
                table: "Penalties",
                newName: "ReturnId");

            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "Checkouts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "Checkouts");

            migrationBuilder.RenameColumn(
                name: "ReturnDate",
                table: "Penalties",
                newName: "IssuedDate");

            migrationBuilder.RenameColumn(
                name: "PenaltyAmount",
                table: "Penalties",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "ReturnId",
                table: "Penalties",
                newName: "PenaltyId");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Penalties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Checkouts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Penalty",
                table: "Checkouts",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
