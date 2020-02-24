using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class CreatedAndModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Tags",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Tags",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Step",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Step",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Step",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Step",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Responsibles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Responsibles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Responsibles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Responsibles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "RequirementTypes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "RequirementTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "RequirementTypes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "RequirementTypes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Requirements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Requirements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Requirements",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Requirements",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "RequirementDefinitions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "RequirementDefinitions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "RequirementDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "RequirementDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Projects",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "PreservationRecords",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "PreservationRecords",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "PreservationRecords",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "PreservationRecords",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "PreservationPeriods",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "PreservationPeriods",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "PreservationPeriods",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "PreservationPeriods",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Persons",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Persons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Persons",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Persons",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Modes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Modes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Modes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Modes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Journeys",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Journeys",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Journeys",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Journeys",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "FieldValues",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "FieldValues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "FieldValues",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "FieldValues",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Fields",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Fields",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Fields",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Fields",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Actions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Actions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Actions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Actions");
        }
    }
}
