using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class TagType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAreaTag",
                table: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "TagType",
                table: "Tags",
                nullable: false,
                defaultValue: "Standard");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_tag_check_valid_tag_type",
                table: "Tags",
                sql: "TagType in ('Standard','PreArea','SiteArea','PoArea')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_tag_check_valid_tag_type",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagType",
                table: "Tags");

            migrationBuilder.AddColumn<bool>(
                name: "IsAreaTag",
                table: "Tags",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
