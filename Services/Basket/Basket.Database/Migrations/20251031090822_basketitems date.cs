using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basket.Database.Migrations
{
    /// <inheritdoc />
    public partial class basketitemsdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                schema: "public",
                table: "BasketItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAdded",
                schema: "public",
                table: "BasketItems");
        }
    }
}
