using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace odev1.Migrations
{
    /// <inheritdoc />
    public partial class trainersaat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "WeekdayEnd",
                table: "Trainers",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WeekdayStart",
                table: "Trainers",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WeekendEnd",
                table: "Trainers",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WeekendStart",
                table: "Trainers",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeekdayEnd",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "WeekdayStart",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "WeekendEnd",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "WeekendStart",
                table: "Trainers");
        }
    }
}
