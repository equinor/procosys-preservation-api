using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RequirementUsage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Usage",
                table: "RequirementDefinitions",
                maxLength: 32,
                nullable: false,
                defaultValue: "ForAll");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_reqdef_check_valid_usage",
                table: "RequirementDefinitions",
                sql: "Usage in ('ForAll','ForSuppliersOnly','ForOtherThanSuppliers')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_reqdef_check_valid_usage",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "Usage",
                table: "RequirementDefinitions");
        }
    }
}
