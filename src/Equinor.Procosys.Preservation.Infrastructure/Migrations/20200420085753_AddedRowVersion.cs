using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddedRowVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Tags",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "TagRequirements",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "TagFunctions",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "TagFunctionRequirements",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Steps",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Responsibles",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "RequirementTypes",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "RequirementDefinitions",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Projects",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PreservationRecords",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PreservationPeriods",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Persons",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Modes",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Journeys",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "FieldValues",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Fields",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Actions",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "TagRequirements");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "TagFunctions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "TagFunctionRequirements");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Actions");
        }
    }
}
