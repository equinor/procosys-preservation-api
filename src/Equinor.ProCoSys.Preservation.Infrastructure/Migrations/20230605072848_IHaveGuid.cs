using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IHaveGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProCoSysGuid",
                table: "Projects",
                newName: "Guid");

            migrationBuilder.RenameColumn(
                name: "ObjectGuid",
                table: "PreservationRecords",
                newName: "Guid");

            migrationBuilder.RenameColumn(
                name: "Oid",
                table: "Persons",
                newName: "Guid");

            migrationBuilder.RenameIndex(
                name: "IX_Persons_Oid",
                table: "Persons",
                newName: "IX_Persons_Guid");

            migrationBuilder.RenameColumn(
                name: "ObjectGuid",
                table: "History",
                newName: "SourceGuid");

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Tags",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "Projects",
                newName: "ProCoSysGuid");

            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "PreservationRecords",
                newName: "ObjectGuid");

            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "Persons",
                newName: "Oid");

            migrationBuilder.RenameIndex(
                name: "IX_Persons_Guid",
                table: "Persons",
                newName: "IX_Persons_Oid");

            migrationBuilder.RenameColumn(
                name: "SourceGuid",
                table: "History",
                newName: "ObjectGuid");
        }
    }
}
