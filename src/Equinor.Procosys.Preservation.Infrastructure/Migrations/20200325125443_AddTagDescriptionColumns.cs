using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddTagDescriptionColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AreaDescription",
                table: "Tags",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisciplineDescription",
                table: "Tags",
                maxLength: 255,
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaDescription",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DisciplineDescription",
                table: "Tags");
        }
    }
}
