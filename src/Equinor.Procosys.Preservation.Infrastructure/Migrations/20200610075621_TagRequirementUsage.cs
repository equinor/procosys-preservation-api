using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class TagRequirementUsage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Usage",
                table: "TagRequirements",
                maxLength: 32,
                nullable: false,
                defaultValue: "ForAll");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_tagreq_check_valid_usage",
                table: "TagRequirements",
                sql: "Usage in ('ForAll','ForSuppliersOnly','ForOtherThanSuppliers')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_tagreq_check_valid_usage",
                table: "TagRequirements");

            migrationBuilder.DropColumn(
                name: "Usage",
                table: "TagRequirements");
        }
    }
}
