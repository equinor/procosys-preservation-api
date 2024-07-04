using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "RequirementDefinitions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Actions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "TagRequirements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "RequirementTypes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "PreservationPeriods",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Fields",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "TagRequirements");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Fields");
        }
    }
}
