using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace odev1.Migrations
{
    /// <inheritdoc />
    public partial class trainerguncel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Trainers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userId",
                table: "Trainers");
        }
    }
}
