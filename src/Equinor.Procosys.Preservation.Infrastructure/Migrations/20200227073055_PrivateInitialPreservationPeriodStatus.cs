using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class PrivateInitialPreservationPeriodStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_requirement_check_valid_initial_status",
                table: "Requirements");

            migrationBuilder.RenameColumn(
                name: "InitialPreservationPeriodStatus",
                table: "Requirements",
                newName: "_initialPreservationPeriodStatus");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_requirement_check_valid_initial_status",
                table: "Requirements",
                sql: "_initialPreservationPeriodStatus in ('NeedsUserInput','ReadyToBePreserved')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_requirement_check_valid_initial_status",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "_initialPreservationPeriodStatus",
                table: "Requirements");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_requirement_check_valid_initial_status",
                table: "Requirements",
                sql: "InitialPreservationPeriodStatus in ('NeedsUserInput','ReadyToBePreserved')");

            migrationBuilder.AddColumn<string>(
                name: "InitialPreservationPeriodStatus",
                table: "Requirements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "NeedsUserInput");
        }
    }
}
