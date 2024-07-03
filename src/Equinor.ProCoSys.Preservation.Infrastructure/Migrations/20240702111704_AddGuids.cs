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
                name: "TagGuid",
                table: "TagRequirements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TagRequirementGuid",
                table: "PreservationPeriods",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TagGuid",
                table: "Actions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagGuid",
                table: "TagRequirements");

            migrationBuilder.DropColumn(
                name: "TagRequirementGuid",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "TagGuid",
                table: "Actions");
        }
    }
}
