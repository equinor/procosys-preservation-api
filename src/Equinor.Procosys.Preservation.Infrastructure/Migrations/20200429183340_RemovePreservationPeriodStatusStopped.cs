using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RemovePreservationPeriodStatusStopped : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_period_check_valid_status",
                table: "PreservationPeriods");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_period_check_valid_status",
                table: "PreservationPeriods",
                sql: "Status in ('NeedsUserInput','ReadyToBePreserved','Preserved')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_period_check_valid_status",
                table: "PreservationPeriods");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_period_check_valid_status",
                table: "PreservationPeriods",
                sql: "Status in ('NeedsUserInput','ReadyToBePreserved','Preserved','Stopped')");
        }
    }
}
