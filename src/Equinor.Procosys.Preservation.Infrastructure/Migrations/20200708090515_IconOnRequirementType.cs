using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class IconOnRequirementType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "RequirementTypes",
                nullable: false,
                maxLength: 32,
                defaultValue: "Other");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_requirement_type_check_icon",
                table: "RequirementTypes",
                sql: "Icon in ('Area','Battery','Bearings','Electrical','Heating','Installation','Measure','Nitrogen','Other','Pressure','Rotate')");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_requirement_type_check_icon",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "RequirementTypes");
        }
    }
}
