using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RemoveRequirementTypeIconMeasure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_requirement_type_check_icon",
                table: "RequirementTypes");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_requirement_type_check_icon",
                table: "RequirementTypes",
                sql: "Icon in ('Other','Area','Battery','Bearings','Electrical','Heating','Installation','Nitrogen','Power','Pressure','Rotate')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_requirement_type_check_icon",
                table: "RequirementTypes");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_requirement_type_check_icon",
                table: "RequirementTypes",
                sql: "Icon in ('Other','Area','Battery','Bearings','Electrical','Heating','Installation','Measure','Nitrogen','Power','Pressure','Rotate')");
        }
    }
}
