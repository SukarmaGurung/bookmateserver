using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server1.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnouncementTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DiscountEndTime",
                table: "Books",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "Books",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DiscountStartTime",
                table: "Books",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountEndTime",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "DiscountStartTime",
                table: "Books");
        }
    }
}
