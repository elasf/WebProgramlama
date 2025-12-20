using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace odev1.Migrations
{
    /// <inheritdoc />
    public partial class guncellll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Trainers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_userId",
                table: "Trainers",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_AspNetUsers_userId",
                table: "Trainers",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_AspNetUsers_userId",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_userId",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Trainers");
        }
    }
}
