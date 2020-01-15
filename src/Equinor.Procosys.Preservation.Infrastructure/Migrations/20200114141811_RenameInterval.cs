using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RenameInterval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "NextDueTime",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagNumber",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Interval",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "DefaultInterval",
                table: "RequirementDefinitions");

            migrationBuilder.AddColumn<string>(
                name: "TagNo",
                table: "Tags",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IntervalWeeks",
                table: "Requirements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefaultIntervalWeeks",
                table: "RequirementDefinitions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagNo",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IntervalWeeks",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "DefaultIntervalWeeks",
                table: "RequirementDefinitions");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tags",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextDueTime",
                table: "Tags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TagNumber",
                table: "Tags",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Interval",
                table: "Requirements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefaultInterval",
                table: "RequirementDefinitions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
