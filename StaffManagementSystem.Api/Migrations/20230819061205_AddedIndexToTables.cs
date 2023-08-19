using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffManagementSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexToTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Vacations_Id",
                table: "Vacations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Kadrs_Id",
                table: "Kadrs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Directors_Id",
                table: "Directors",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_Id",
                table: "Admins",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vacations_Id",
                table: "Vacations");

            migrationBuilder.DropIndex(
                name: "IX_Users_Id",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Kadrs_Id",
                table: "Kadrs");

            migrationBuilder.DropIndex(
                name: "IX_Directors_Id",
                table: "Directors");

            migrationBuilder.DropIndex(
                name: "IX_Admins_Id",
                table: "Admins");
        }
    }
}
