using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace odev1.Migrations
{
    /// <inheritdoc />
    public partial class AddAIRecommendation : Migration
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

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Services",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "AIRecommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    MemberId = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<decimal>(type: "numeric", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    BodyType = table.Column<string>(type: "text", nullable: true),
                    Goal = table.Column<string>(type: "text", nullable: true),
                    PhotoPath = table.Column<string>(type: "text", nullable: true),
                    ExerciseRecommendations = table.Column<string>(type: "text", nullable: true),
                    DietRecommendations = table.Column<string>(type: "text", nullable: true),
                    GeneralAdvice = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIRecommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIRecommendations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AIRecommendations_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProgressEntries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userId = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    weightKg = table.Column<decimal>(type: "numeric", nullable: false),
                    bodyFatPercent = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressEntries", x => x.id);
                    table.ForeignKey(
                        name: "FK_ProgressEntries_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIRecommendations_MemberId",
                table: "AIRecommendations",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_AIRecommendations_UserId",
                table: "AIRecommendations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressEntries_userId",
                table: "ProgressEntries",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_Trainers_trainerId",
                table: "Availabilities",
                column: "trainerId",
                principalTable: "Trainers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_Trainers_trainerId",
                table: "Availabilities");

            migrationBuilder.DropTable(
                name: "AIRecommendations");

            migrationBuilder.DropTable(
                name: "ProgressEntries");

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

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Services",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);

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
