using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RenameSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "Tags",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "TagFunctions",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "TagFunctionRequirements",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "Steps",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "Responsibles",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "RequirementTypes",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "Requirements",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "RequirementDefinitions",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "Projects",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "PreservationRecords",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "PreservationPeriods",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "Modes",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "Journeys",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "FieldValues",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "Fields",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "Schema",
                table: "Actions",
                newName: "Plant");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "Tags",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "TagFunctions",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "TagFunctionRequirements",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "Steps",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "Responsibles",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "RequirementTypes",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "Requirements",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "RequirementDefinitions",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "Projects",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "PreservationRecords",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "PreservationPeriods",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "Modes",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "Journeys",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "FieldValues",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "Fields",
                newName: "Schema");

            migrationBuilder.RenameColumn(
                name: "Plant",
                table: "Actions",
                newName: "Schema");
        }
    }
}
