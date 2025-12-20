using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace odev1.Migrations
{
    /// <inheritdoc />
    public partial class addmigrationgaranti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Trainers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Trainers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
