using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class Renamed_NeedsUserInput : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_period_check_valid_status",
                table: "PreservationPeriods");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_period_check_valid_status",
                table: "PreservationPeriods",
                sql: "Status in ('NeedsUserInput','ReadyToBePreserved','Preserved','Stopped')");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PreservationPeriods",
                nullable: false,
                defaultValue: "NeedsUserInput",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "NeedUserInput");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_period_check_valid_status",
                table: "PreservationPeriods");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_period_check_valid_status",
                table: "PreservationPeriods",
                sql: "Status in ('NeedUserInput','ReadyToBePreserved','Preserved','Stopped')");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PreservationPeriods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "NeedUserInput",
                oldClrType: typeof(string),
                oldDefaultValue: "NeedsUserInput");
        }
    }
}
