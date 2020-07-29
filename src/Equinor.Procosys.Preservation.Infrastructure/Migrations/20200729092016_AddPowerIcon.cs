using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddPowerIcon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_requirement_type_check_icon",
                table: "RequirementTypes");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_requirement_type_check_icon",
                table: "RequirementTypes",
                sql: "Icon in ('Other','Area','Battery','Bearings','Electrical','Heating','Installation','Measure','Nitrogen','Power','Pressure','Rotate')");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "RequirementTypes",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldDefaultValue: "Other");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_requirement_type_check_icon",
                table: "RequirementTypes");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_requirement_type_check_icon",
                table: "RequirementTypes",
                sql: "Icon in ('Area','Battery','Bearings','Electrical','Heating','Installation','Measure','Nitrogen','Other','Pressure','Rotate')");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "RequirementTypes",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Other",
                oldClrType: typeof(string),
                oldMaxLength: 32);
        }
    }
}
