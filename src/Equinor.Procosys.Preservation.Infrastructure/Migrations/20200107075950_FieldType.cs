using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class FieldType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FieldType",
                table: "Fields",
                nullable: false,
                defaultValue: "Info");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_field_check_valid_fieldtype",
                table: "Fields",
                sql: "FieldType in ('Info','Number','CheckBox','Attachment')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_field_check_valid_fieldtype",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "Fields");
        }
    }
}
