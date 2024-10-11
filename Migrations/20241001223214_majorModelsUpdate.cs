using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiberaryManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class majorModelsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Members_MemberId",
                table: "Checkouts");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Checkouts_MemberId",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Checkouts");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Checkouts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_ApplicationUserId",
                table: "Checkouts",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_AspNetUsers_ApplicationUserId",
                table: "Checkouts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_AspNetUsers_ApplicationUserId",
                table: "Checkouts");

            migrationBuilder.DropIndex(
                name: "IX_Checkouts_ApplicationUserId",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Checkouts");

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Checkouts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MembershipDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_Members_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_MemberId",
                table: "Checkouts",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_ApplicationUserId",
                table: "Members",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Members_MemberId",
                table: "Checkouts",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
