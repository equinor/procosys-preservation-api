using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class CreatedAndModified2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Actions");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Tags",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Step",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Step",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Responsibles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Responsibles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "RequirementTypes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "RequirementTypes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Requirements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Requirements",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "RequirementDefinitions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "RequirementDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Projects",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "PreservationRecords",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "PreservationRecords",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "PreservationPeriods",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "PreservationPeriods",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Persons",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Persons",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Modes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Modes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Journeys",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Journeys",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "FieldValues",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "FieldValues",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Fields",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Fields",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Actions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Actions");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Tags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Tags",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Step",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Step",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Responsibles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Responsibles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "RequirementTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "RequirementTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Requirements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Requirements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "RequirementDefinitions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "RequirementDefinitions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "PreservationRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "PreservationRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "PreservationPeriods",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "PreservationPeriods",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Persons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Persons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Modes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Modes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Journeys",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Journeys",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "FieldValues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "FieldValues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Fields",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Fields",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Actions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Actions",
                type: "datetime2",
                nullable: true);
        }
    }
}
