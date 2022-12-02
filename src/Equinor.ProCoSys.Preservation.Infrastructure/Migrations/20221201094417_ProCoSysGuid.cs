using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    public partial class ProCoSysGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CommPkgProCoSysGuid",
                table: "Tags",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "McPkgProCoSysGuid",
                table: "Tags",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProCoSysGuid",
                table: "Tags",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommPkgProCoSysGuid",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "McPkgProCoSysGuid",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ProCoSysGuid",
                table: "Tags");
        }
    }
}
