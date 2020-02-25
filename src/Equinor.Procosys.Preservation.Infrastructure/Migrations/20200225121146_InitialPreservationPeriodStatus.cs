using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class InitialPreservationPeriodStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InitialPreservationPeriodStatus",
                table: "Requirements",
                nullable: false,
                defaultValue: "NeedsUserInput");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_requirement_check_valid_initial_status",
                table: "Requirements",
                sql: "InitialPreservationPeriodStatus in ('NeedsUserInput','ReadyToBePreserved')");

            migrationBuilder.Sql(
                @"");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_requirement_check_valid_initial_status",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "InitialPreservationPeriodStatus",
                table: "Requirements");
        }
    }
}
