using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiberaryManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class changePenalitiesSetName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_Checkouts_CheckoutId",
                table: "Penalties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Penalties",
                table: "Penalties");

            migrationBuilder.RenameTable(
                name: "Penalties",
                newName: "Returns");

            migrationBuilder.RenameIndex(
                name: "IX_Penalties_CheckoutId",
                table: "Returns",
                newName: "IX_Returns_CheckoutId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Returns",
                table: "Returns",
                column: "ReturnId");

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_Checkouts_CheckoutId",
                table: "Returns",
                column: "CheckoutId",
                principalTable: "Checkouts",
                principalColumn: "CheckoutId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Returns_Checkouts_CheckoutId",
                table: "Returns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Returns",
                table: "Returns");

            migrationBuilder.RenameTable(
                name: "Returns",
                newName: "Penalties");

            migrationBuilder.RenameIndex(
                name: "IX_Returns_CheckoutId",
                table: "Penalties",
                newName: "IX_Penalties_CheckoutId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Penalties",
                table: "Penalties",
                column: "ReturnId");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_Checkouts_CheckoutId",
                table: "Penalties",
                column: "CheckoutId",
                principalTable: "Checkouts",
                principalColumn: "CheckoutId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
