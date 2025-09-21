using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Database.PostgresSql.Migrations
{
    /// <inheritdoc />
    public partial class addcatalogslugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                schema: "public",
                table: "Catalogs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                schema: "public",
                table: "Catalogs");
        }
    }
}
