using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace odev1.Migrations
{
    /// <inheritdoc />
    public partial class AddMembersAndFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_Trainers_trainerid",
                table: "Availabilities");

            migrationBuilder.DropColumn(
                name: "tarinerId",
                table: "Availabilities");

            migrationBuilder.RenameColumn(
                name: "duraiton",
                table: "Services",
                newName: "duration");

            migrationBuilder.RenameColumn(
                name: "trainerid",
                table: "Availabilities",
                newName: "trainerId");

            migrationBuilder.RenameIndex(
                name: "IX_Availabilities_trainerid",
                table: "Availabilities",
                newName: "IX_Availabilities_trainerId");

            /*migrationBuilder.CreateIndex(
                name: "IX_Trainers_userId",
                table: "Trainers",
                column: "userId");*/

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_Trainers_trainerId",
                table: "Availabilities",
                column: "trainerId",
                principalTable: "Trainers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            /*migrationBuilder.AddForeignKey(
                name: "FK_Trainers_AspNetUsers_userId",
                table: "Trainers",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_Trainers_trainerId",
                table: "Availabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_AspNetUsers_userId",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_userId",
                table: "Trainers");

            migrationBuilder.RenameColumn(
                name: "duration",
                table: "Services",
                newName: "duraiton");

            migrationBuilder.RenameColumn(
                name: "trainerId",
                table: "Availabilities",
                newName: "trainerid");

            migrationBuilder.RenameIndex(
                name: "IX_Availabilities_trainerId",
                table: "Availabilities",
                newName: "IX_Availabilities_trainerid");

            migrationBuilder.AddColumn<int>(
                name: "tarinerId",
                table: "Availabilities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_Trainers_trainerid",
                table: "Availabilities",
                column: "trainerid",
                principalTable: "Trainers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
